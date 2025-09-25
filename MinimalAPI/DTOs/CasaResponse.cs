using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinimalApi.Dtos;

public readonly record struct CasaResponse(int IdCasa, string Direccion);