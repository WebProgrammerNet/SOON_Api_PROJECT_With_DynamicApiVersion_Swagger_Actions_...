using ApiProjectModul.AppDataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiProjectModul.Services
{
    public interface ISeedDataBaseErrorService
    {
        Task Initialize(AppDataBase context);
    }
}
