﻿using PropertyManagerFL.Application.ViewModels;
using PropertyManagerFL.Application.ViewModels.LookupTables;

namespace PropertyManagerFL.Application.Interfaces.Services.AppManager;
public interface IDistritosConcelhosService
{
    Task<IEnumerable<DistritoConcelho>> GetConcelhosByDistrito(int id);
    Task<IEnumerable<DistritoConcelho>> GetConcelhos();
    Task<IEnumerable<LookupTableVM>> GetDistritos();
    Task<bool> UpdateCoeficienteIMI(int Id, float coeficienteIMI);
}
