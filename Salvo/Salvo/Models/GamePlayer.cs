using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salvo.Models
{
    
    public class GamePlayer
    {
        public long Id { get; set; }
        public long GameId { get; set; }
        public Game Game { get; set; }
        public long PlayerId { get; set; }
        public Player Player { get; set; }
        public DateTime? JoinDate { get; set; }

        public ICollection<Ship> Ships { get; set; }
        public ICollection<Salvo> Salvos { get; set; }


        public Score GetScore()
        {
            return Player.GetScore(Game);
        }

        public GamePlayer GetOpponent()
        {
            return Game.GamePlayers.FirstOrDefault(gp => gp.Id != Id);
        }

        public ICollection<SalvoHitDTO> GetHits()
        {
            return Salvos.Select(salvo => new SalvoHitDTO
            {
                Turn = salvo.Turn,
                Hits = GetOpponent()?.Ships.Select(ship => new ShipHitDTO
                {
                    Type= ship.Type,
                    Hits = salvo.Locations.Where
                    (salvolocation => ship.Locations.Any(shipLocation => shipLocation.Location == salvolocation.Location))
                    .Select(salvoLocation => salvoLocation.Location).ToList()
                }).ToList()
            }).ToList();
        }

        public ICollection<String> GetSunks()
        {
            int lastTurn = Salvos.Count;
            List<String> salvoLocations = 
                GetOpponent()?.Salvos
                .Where(salvo=> salvo.Turn <= lastTurn)
                .SelectMany(salvo => salvo.Locations.Select(location => location.Location)).ToList();
                
                
            return Ships?.Where (ship => ship.Locations.Select(shipLocation => shipLocation.Location)
                         .All(shipLocation => salvoLocations != null ? salvoLocations.Any(salvoLocation => salvoLocation == shipLocation) : false)
                                )
                         .Select(ship=>ship.Type).ToList();
        }

        public GameState GetGameState()
        {
            GameState gamestate = GameState.ENTER_SALVO;
           
            var opponent = GetOpponent();

            var ships = Ships.Count();
            var shipsOpponent = opponent?.Ships.Count();

            var salvos = Salvos.Count();
            var salvosOpponent = opponent?.Salvos.Count();

            var sunks = GetSunks().Count();
            var sunksOpponent = opponent?.GetSunks().Count();

            //No tengo oponente
            if(opponent == null)
            {
                gamestate = GameState.PLACE_SHIPS;
                //No tengo oponente pero sí puse los barcos
                if(ships == 5)
                {
                    gamestate = GameState.WAIT;
                }
            }
            //Tengo oponente, puse mís barcos pero él no
            if(ships == 5 && shipsOpponent == 0)
            {
                gamestate = GameState.WAIT;
            }


            //Mi oponente y yo posicionamos los barcos
            if (ships == 5 && shipsOpponent == 5)
            { 
                //Posicionamos los barcos y le toca al que entro antes
                if(JoinDate < opponent.JoinDate)
                {
                    gamestate = GameState.PLACE_SHIPS;
                }
                //Entre segundo, por lo tanto tengo que esperar
                else
                {
                    gamestate = GameState.WAIT;
                }
                
            }

            //Posicione los barcos y dispare, pero no es mí turno
            if (salvos > salvosOpponent)
            {
                gamestate = GameState.WAIT;
            }
            //Posicione los barcos y el oponente disparo, pero tenemos los mismos salvos
            if (salvos == salvosOpponent)
            {
                //Posicione los barcos y el oponente disparo, entonces es el turno del que entro antes
                if (JoinDate < opponent.JoinDate)
                {
                    gamestate = GameState.ENTER_SALVO;
                }
                //Posicione los barcos y dispare, y es el turno del oponente
                else
                {
                    gamestate = GameState.WAIT;
                }
            }


            if (sunks == 5)
            {
                gamestate = GameState.WIN;
            }
            if(sunksOpponent == 5)
            {
                gamestate = GameState.LOSS;
            }
            if(sunks == 5 && sunksOpponent == 5)
            {
                gamestate = GameState.TIE;
            }
            
            

            return gamestate;
        }

    }

}

