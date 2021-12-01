﻿using System;
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
                         .All(salvoLocation => salvoLocations != null ? salvoLocations.Any(shipLocation => shipLocation == salvoLocation) : false)
                                )
                         .Select(ship=>ship.Type).ToList();
        }

    }

}

