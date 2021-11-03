﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salvo.Models
{
    public static class DbInitializer
    {
        public static void Initialize(SalvoContext context ) 
        {
           
            if (!context.Players.Any())
            {

                var Players = new Player[]
                {
                    new Player { Name = "Jack Bauer", Email = "j.bauer@ctu.gov", Password = "24" },
                    new Player { Name = "Chloe O'Brian", Email = "c.obrian@ctu.gov", Password = "42" },
                    new Player { Name = "Kim Bauer", Email = "kim_bauer@gmail.com", Password = "kb" },
                    new Player { Name = "Tony Almeida", Email = "t.almeida@ctu.gov", Password = "mole" }
                };
                    foreach (Player p in Players)
                     {
                        context.Players.Add(p);
                     }
                     context.SaveChanges();

            }

            if (!context.Games.Any())
            {
                var Games = new Game[]
                {
                    new Game{CreationDate = DateTime.Now },
                    new Game{CreationDate = DateTime.Now.AddHours(1) },
                    new Game{CreationDate = DateTime.Now.AddHours(2) },
                    new Game{CreationDate = DateTime.Now.AddHours(3) },
                    new Game{CreationDate = DateTime.Now.AddHours(4) },
                    new Game{CreationDate = DateTime.Now.AddHours(5) },
                    new Game{CreationDate = DateTime.Now.AddHours(6) },
                    new Game{CreationDate = DateTime.Now.AddHours(7) },
                };

                foreach(Game g in Games)
                {
                    context.Games.Add(g);
                }
                context.SaveChanges();
            }

            if (!context.GamePlayers.Any())
            {


                var GamePlayers = new GamePlayer[]
                {
                    new GamePlayer{Game = context.Games.Find(1L), Player = context.Players.Find(1L), JoinDate = DateTime.Now },
                    new GamePlayer{Game = context.Games.Find(1L), Player = context.Players.Find(2L), JoinDate = DateTime.Now },

                    new GamePlayer{Game = context.Games.Find(2L), Player = context.Players.Find(1L), JoinDate = DateTime.Now },
                    new GamePlayer{Game = context.Games.Find(2L), Player = context.Players.Find(2L), JoinDate = DateTime.Now },

                    new GamePlayer{Game = context.Games.Find(3L), Player = context.Players.Find(2L), JoinDate = DateTime.Now },
                    new GamePlayer{Game = context.Games.Find(3L), Player = context.Players.Find(4L), JoinDate = DateTime.Now },

                    new GamePlayer{Game = context.Games.Find(4L), Player = context.Players.Find(2L), JoinDate = DateTime.Now },
                    new GamePlayer{Game = context.Games.Find(4L), Player = context.Players.Find(1L), JoinDate = DateTime.Now },

                    new GamePlayer{Game = context.Games.Find(5L), Player = context.Players.Find(4L), JoinDate = DateTime.Now },
                    new GamePlayer{Game = context.Games.Find(5L), Player = context.Players.Find(1L), JoinDate = DateTime.Now },

                    new GamePlayer{Game = context.Games.Find(6L), Player = context.Players.Find(3L), JoinDate = DateTime.Now },


                    new GamePlayer{Game = context.Games.Find(7L), Player = context.Players.Find(4L), JoinDate = DateTime.Now },


                    new GamePlayer{Game = context.Games.Find(8L), Player = context.Players.Find(3L), JoinDate = DateTime.Now },
                    new GamePlayer{Game = context.Games.Find(8L), Player = context.Players.Find(4L), JoinDate = DateTime.Now },
                };

                foreach(GamePlayer g in GamePlayers)
                {
                    context.GamePlayers.Add(g);
                }

                context.SaveChanges();
            }



           
        }
    }
}