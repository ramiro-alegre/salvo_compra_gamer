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
            var opponent = GetOpponent();

            var ships = Ships.Count;
            var shipsOpponent = opponent?.Ships.Count;

            var salvos = Salvos.Count;
            var salvosOpponent = opponent?.Salvos.Count;

            var sunks = GetSunks().Count;
            var sunksOpponent = opponent?.GetSunks().Count;
            //No tengo oponente
            if (opponent == null)
            {
                return GameState.PLACE_SHIPS;
            }

            //No posicione los barcos, por lo tanto, tengo que hacerlo
            if (ships == 0)
            {
              return  GameState.PLACE_SHIPS;
            }


            //Yo tengo más salvos, por lo tanto, tengo que esperar
            if (salvos > salvosOpponent)
            {
                return GameState.WAIT;
            }
            //Mí oponente tiene más salvos, por lo tanto, tengo que disparar
            else if(salvos < salvosOpponent)
            {
                return GameState.ENTER_SALVO;
            }
            //Los 2 tenemos los mismos salvos
            else 
            {
                if(ships == sunks && shipsOpponent == sunksOpponent)
                {
                    return GameState.TIE;
                }
                else if(sunks == ships)
                {
                    return GameState.LOSS;
                }
                else if(shipsOpponent == sunksOpponent)
                {
                    return GameState.WIN;
                }
                else if(JoinDate < opponent.JoinDate)
                {
                    return GameState.ENTER_SALVO;
                }
                else
                {
                    return GameState.WAIT;
                }
            }
        }

    }

}

