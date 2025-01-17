﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tiled;
using System;
using System.Linq;

namespace Jeux.Perso
{
    class Enemy : Sprite
    {
        public Enemy(AnimatedSprite texture) 
            : base(texture)
        {
            Health = 5;
        }

        //deplacement et echelles
        private Vector2 deplacement = Vector2.Zero;
        bool echelleHaut = false, echelleBas = false, right = false;

        //bords
        private enum LastBord { droite, gauche, aucun };
        private LastBord last = LastBord.aucun;
        bool toucheBordFenetreDroite ;
        bool toucheBordFenetreGauche ;

        public bool _visible = true, _spam = false, _detection = false;

        public void Move(GameTime gameTime, TiledMap _map, string layerCollision, string layerClimb, GraphicsDevice graphicsDevice, Sprite player)
        {
            //vitesse ennemi ( à changer j'en avais juste marre qu'il aille lentement )
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float walkSpeed = elapsedTime * 100;

            //position ennemi
            float positionColonnePerso = (Position.X / _map.TileWidth);
            float positionLignePerso = ((Position.Y + Texture.TextureRegion.Height / 2) / _map.TileHeight);

            //timer?
            int timer = 1;


            Velocity.X = 0;

            //touche bord fenetre ou plus de sol
            if (Position.X + Texture.TextureRegion.Width / 2 <= 0 || !IsCollision(positionColonnePerso-1, positionLignePerso, layerCollision, _map))
            {
                toucheBordFenetreGauche = true;
            }
            else if (Position.X + Texture.TextureRegion.Width /2 >= Game1.ScreenWidth || !IsCollision(positionColonnePerso+1, positionLignePerso, layerCollision, _map))
            {
                toucheBordFenetreDroite = true;
            }

            if (toucheBordFenetreDroite == true)
            {
                toucheBordFenetreDroite = false;
                last = LastBord.droite;
            }
            else if(toucheBordFenetreGauche == true)
            {
                toucheBordFenetreGauche = false;
                last = LastBord.gauche;
            }

            //deplacement en fonction bord fenêtre/sol/echelle (monté si perso au dessus à revoir )
            if (last == LastBord.droite && IsCollision(positionColonnePerso, positionLignePerso, layerCollision, _map) && !echelleHaut && !echelleBas)
            {
                deplacement = -Vector2.UnitX;
            }
            else if (last == LastBord.gauche && IsCollision(positionColonnePerso, positionLignePerso, layerCollision, _map) && !echelleHaut && !echelleBas)
            {
                deplacement = Vector2.UnitX;
            }
            else if (last == LastBord.droite && !echelleHaut && !echelleBas)
            {
                deplacement = -Vector2.UnitX;
            }
            else if (last == LastBord.gauche && !echelleHaut && !echelleBas)
            {
                deplacement = Vector2.UnitX;
            }
            else if (echelleBas == true)
            {
                deplacement = Vector2.UnitY;
            }
            else if (echelleHaut == true)
            {
                deplacement = -Vector2.UnitY;
            }

            int end = (int)player.Position.Y + 5;
            int start = (int)player.Position.Y - 10;

            if (Enumerable.Range(start, end).Contains(Rectangle.Y))
                _detection = true;


            // detection echelle 

            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyUp(Keys.X))
                _spam = false;

            //mouvement si le joueur est detecter
            if (_detection)
            {
                walkSpeed = elapsedTime * 200;
                if (Enumerable.Range(start, end).Contains(Rectangle.Y))
                {
                    if (Math.Abs(Position.X - player.Position.X) < 5 && keyboardState.IsKeyDown(Keys.X) && !_spam) // si le joueur est proche de l'ennemi et qu'il appui sur x
                    {
                        Health--;
                        deplacement = Vector2.Zero;
                        _spam = true;
                    }
                    else if (Math.Abs(this.Position.X - player.Position.X) < 2 && keyboardState.IsKeyUp(Keys.X) && timer > 0) // si le joueur est proche de l'ennemi et qu'il n'appui PAS sur x
                    {
                        hit = true; // le joueur est frappé
                        deplacement = Vector2.Zero;
                        if(right)
                            AnimationE = TypeAnimationEnnemi.enemyHitRight;
                        else
                            AnimationE = TypeAnimationEnnemi.enemyHitLeft;
                        timer -= (int)elapsedTime; // pour eviter que le joueur se fasse tué en 10ms
                    }
                    else if (Position.X > player.Position.X)
                    {
                        deplacement = -Vector2.UnitX;
                    }
                    else if (Position.X < player.Position.X)
                    {
                        deplacement = Vector2.UnitX;
                    }
                }
                else if (player.Position.Y > this.Position.Y) //si player en dessous d'ennemie
                {
                    if (IsCollision(positionColonnePerso, positionLignePerso, layerClimb, _map) && IsCollision(positionColonnePerso, positionLignePerso + 1, layerClimb, _map) && !echelleHaut)
                    {
                        echelleBas = true;
                    }
                    else
                        echelleBas = false;
                }
                if (player.Position.Y < this.Position.Y) //si player au dessus d'en
                {
                    if (IsCollision(positionColonnePerso, positionLignePerso - 1, layerClimb, _map) && IsCollision(positionColonnePerso, positionLignePerso - 1, layerClimb, _map) && !echelleBas)
                    {
                        echelleHaut = true;
                    }
                    else
                        echelleHaut = false;
                }
            }

            hit = false;

            //AnimationE
            if (deplacement == -Vector2.UnitX)
            {
                right = false;
                AnimationE = TypeAnimationEnnemi.enemyWalkLeft;
            }

            if (deplacement == Vector2.UnitX)
            {
                right = true;
                AnimationE = TypeAnimationEnnemi.enemyWalkRight;
            }

            //deplacement
            if (IsCollision(positionColonnePerso, positionLignePerso, layerCollision, _map) 
                && toucheBordFenetreGauche == false  
                && toucheBordFenetreDroite == false)
            {
                Position += walkSpeed * deplacement;
            }

            //gravite
            if ((!IsCollision(positionColonnePerso-1, positionLignePerso, layerCollision, _map)))
            {
                Velocity.Y += Gravity.Y * elapsedTime;
            }
            else
                Velocity.Y = 0;

            Position += Velocity * elapsedTime;

            //affichage
            this.Texture.Play(this.AnimationE.ToString());
            this.Texture.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

        }

        public override void Update(GameTime gameTime, TiledMap _map, string layerCollision, string layerClimb, GraphicsDevice graphicsDevice, Sprite player)
        {
            Move(gameTime, _map, layerCollision, layerClimb, graphicsDevice, player);
        }

        private bool IsCollision(float x, float y, string layer, TiledMap _map)
        {
            TiledMapTile? tile;
            TiledMapTileLayer _obstacleLayer = _map.GetLayer<TiledMapTileLayer>(layer);
            _obstacleLayer = _map.GetLayer<TiledMapTileLayer>(layer);
            if (_obstacleLayer.TryGetTile((ushort)x, (ushort)y, out tile) == false)
            {
                return false;
            }
            if (!tile.Value.IsBlank)
            {
                return true;
            }
            return false;
        }
    }

}
