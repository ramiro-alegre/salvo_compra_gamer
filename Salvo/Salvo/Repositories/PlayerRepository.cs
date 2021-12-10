﻿using Salvo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salvo.Repositories
{
    public class PlayerRepository : RepositoryBase<Player>, IPlayerRepository
    {
       
        public PlayerRepository(SalvoContext repositoryContext) : base(repositoryContext)
        {
            
        }
        public Player FindByMail(string email)
        {
            return FindByCondition(player => player.Email == email).FirstOrDefault();
        }

        public void Save(Player player)
        {
            if (player.Id == 0)
                Create(player);
            else
                Update(player);
            SaveChanges();
        }
    }
}
