﻿using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using Jeux.Perso;
using System.Collections.Generic;
using System;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Serialization;
using Sprite = Jeux.Perso.Sprite;
using MonoGame.Extended.Content;

namespace Jeux.Screen
{
    public class Level : GameScreen
    {
        private Game1 _game1; // pour récupérer la fenêtre de jeu principale

        private List<TiledMap> _map = new List<TiledMap>(); 

        private List<TiledMapRenderer> _renduMap = new List<TiledMapRenderer>();

        private int _mapEnCour;

        private List<Rectangle> _spawn = new List<Rectangle>(), _start = new List<Rectangle>();

        private List<Sprite> _sprites;

        bool _switch = true;

        public Level(Game1 game) : base(game)
        {
            _game1 = game;
        }

        public override void Initialize()
        {
            // TODO: Add your initialization logic here

            _game1.IsMouseVisible = false;

            _mapEnCour = 0;

            base.Initialize();

        }
        public override void LoadContent()
        {

          for (int i = 0; i < 5; i++)
           {
                _map.Add(Content.Load<TiledMap>($"map/{i + 1}Eta"));
                _renduMap.Add(new TiledMapRenderer(GraphicsDevice, _map[i]));
           }

            var player = Content.Load<SpriteSheet>("perso.sf", new JsonContentLoader());
            var playerTexture = new AnimatedSprite(player);
            var enemy = Content.Load<SpriteSheet>("test/enemy.sf", new JsonContentLoader());
            var enemyTexture = new AnimatedSprite(enemy);

            _sprites = new List<Sprite>()
           {
               new Player(playerTexture)
               {
                   Position = new Vector2(10, 10),
               },
               new Enemy(enemyTexture)
               {
                   Position = Vector2.Zero,
               }
           };

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {

            if (_switch)
            {
                int i = 2;
                int y = 2;
                while (i > 0)
                {
                    _start.Add(new Rectangle((int)_map[_mapEnCour].ObjectLayers[y].Objects[0].Position.X,
                                             (int)_map[_mapEnCour].ObjectLayers[y].Objects[0].Position.Y,
                                             (int)_map[_mapEnCour].ObjectLayers[y].Objects[0].Size.Width,
                                             (int)_map[_mapEnCour].ObjectLayers[y].Objects[0].Size.Height));
                    y++;
                    i--;
                }
                _switch = false;
            }


            if (_sprites[0].Rectangle.Intersects(_start[0]))
            {
                _mapEnCour++;
                _switch = true;            
            }

            foreach (Sprite sprite in _sprites)
                sprite.Update(gameTime, _map[_mapEnCour], "sol", "echelles", GraphicsDevice, _sprites[0]);
        }

        public override void Draw(GameTime gameTime)
        {
            _game1.GraphicsDevice.Clear(Color.Red);

            _game1.SpriteBatch.Begin();

            foreach (var sprite in _sprites)
                sprite.Draw(_game1.SpriteBatch);
            
            if(_mapEnCour < 2)
                _renduMap[_mapEnCour].Draw();

            _game1.SpriteBatch.End();
        }
    }
}
