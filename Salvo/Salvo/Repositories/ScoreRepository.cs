using Salvo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salvo.Repositories
{
    public class ScoreRepository : RepositoryBase<Score>, IScoreRepository
    {

        public ScoreRepository(SalvoContext Repositorycontext) : base(Repositorycontext)
        {

        }
        public void Save(Score score)
        {
            Create(score);
            SaveChanges();
        }
    }
}
