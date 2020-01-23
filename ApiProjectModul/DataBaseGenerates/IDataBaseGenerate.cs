using ApiProjectModul.Models;
using ApiProjectModul.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiProjectModul.DataBaseGenerates
{
    public interface IDataBaseGenerate
    {
        IQueryable<Composition> GetAll(QueryParameters queryParameters);
        Composition GetSingle(int id);
        void Add(Composition item);
        void Delete(int id);
        Composition Update(int id, Composition item);
        ICollection<Composition> GetRandomMeal();
        int Count();
        bool Save();
    }
}
