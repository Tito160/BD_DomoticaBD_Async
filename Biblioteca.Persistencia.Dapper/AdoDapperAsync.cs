using System.Data;
using System.Threading.Tasks;
using Dapper;


namespace Biblioteca.Persistencia.Dapper
{
    public class AdoDapperAsync : IAdoAsync
    {
    readonly IDbConnection _conexion;
        private readonly string _queryElectrodomestico
        = @"SELECT  *
        FROM    Electrodomestico
        WHERE   idElectrodomestico = @id;

        SELECT  *
        FROM    HistorialRegistro
        WHERE   idElectrodomestico = @id;";

        private readonly string _queryCasa
        = @"SELECT *
            FROM Casa
            WHERE idCasa = @id;
            
            SELECT  *
            FROM    Electrodomestico
            WHERE   idCasa = @id;";

        private readonly string _queryUsuario
        = @"SELECT *
            FROM Usuario
            WHERE Correo = @Correo and Contrasenia = SHA2(@Contrasenia, 256);";

        public AdoDapperAsync(IDbConnection conexion)
        => _conexion = conexion;
        public async Task AltaCasa(Casa casa)
        {
            var parametros = new DynamicParameters();
            parametros.Add("@unidCasa", direction: ParameterDirection.Output);
            parametros.Add("@unDireccion", casa.Direccion);

            await _conexion.ExecuteAsync("altaCasa", parametros); // Carga el sp y los parametros desde dapper.

            casa.IdCasa = parametros.Get<int>("@unidCasa");
        }

        public async Task AltaConsumo(Consumo consumo)
        {
            var parametros = new DynamicParameters();
            parametros.Add("@unidConsumo", direction: ParameterDirection.Output);
            parametros.Add("@unidElectrodomestico", consumo.IdElectrodomestico);
            parametros.Add("@uninicio", consumo.Inicio);
            parametros.Add("@unDuracion", consumo.Duracion);
            parametros.Add("@unConsumoTotal", consumo.ConsumoTotal);

            await _conexion.ExecuteAsync("altaConsumo", parametros);

            consumo.IdConsumo = parametros.Get<int>("@unidConsumo");
        }

        public async Task AltaElectrodomestico(Electrodomestico electrodomestico)
        {
            var parametros = new DynamicParameters();
            parametros.Add("@unidElectrodomestico", direction: ParameterDirection.Output);
            parametros.Add("@unidCasa", electrodomestico.IdCasa);
            parametros.Add("@unNombre", electrodomestico.Nombre);
            parametros.Add("@unTipo", electrodomestico.Tipo);
            parametros.Add("@unUbicacion", electrodomestico.Ubicacion);
            parametros.Add("@unEncendido", electrodomestico.Encendido);
            parametros.Add("@unApagado", electrodomestico.Apagado);

            await _conexion.ExecuteAsync("altaElectrodomestico", parametros);

            electrodomestico.IdElectrodomestico = parametros.Get<int>("@unidElectrodomestico");
        }

        public async Task AltaHistorialRegistro(HistorialRegistro historialRegistro)
        {
            var parametros = new DynamicParameters();
            parametros.Add("@unidElectrodomestico", historialRegistro.IdElectrodomestico);
            parametros.Add("@unFechaHoraRegistro", historialRegistro.FechaHoraRegistro);

            await _conexion.ExecuteAsync("altaHistorialRegistro", parametros, commandType: CommandType.StoredProcedure);
        }

        public async Task AltaUsuario(Usuario usuario)
        {
            var parametros = new DynamicParameters();
            parametros.Add("@unidUsuario", direction: ParameterDirection.Output);
            parametros.Add("@unNombre", usuario.Nombre);
            parametros.Add("@unCorreo", usuario.Correo);
            parametros.Add("@uncontrasenia", usuario.Contrasenia);
            parametros.Add("@unTelefono", usuario.Telefono);

            await _conexion.ExecuteAsync("altaUsuario", parametros);

            usuario.IdUsuario = parametros.Get<int>("@unidUsuario");
        }

        public async Task<Casa>? ObtenerCasa(int idCasa)
        {
            using (var registro = await _conexion.QueryMultipleAsync(_queryCasa, new { id = idCasa }))
            {
                var casa = await registro.ReadSingleOrDefaultAsync<Casa>();
                if (casa is not null)
                {
                    casa.Electros = await registro.ReadAsync<Electrodomestico>();
                }
            return casa;
            }
        }

        public async Task<Electrodomestico>? ObtenerElectrodomestico(int idElectrodomestico)
        {
            using (var registro =await _conexion.QueryMultipleAsync(_queryElectrodomestico, new { id = idElectrodomestico }))
            {
                var electrodomestico =await registro.ReadSingleOrDefaultAsync<Electrodomestico>();
                if (electrodomestico is not null)
                {
                    var PasarALista = await registro.ReadAsync<HistorialRegistro>();
                    electrodomestico.ConsumoMensual = PasarALista.ToList();
                }
                return electrodomestico;
            }
        }

        public async Task<Usuario>? UsuarioPorPass(string Correo, string Contrasenia)
        {
            var usuario = await _conexion.QueryFirstOrDefaultAsync<Usuario>(_queryUsuario, new {Correo,Contrasenia });
            
            return usuario;
        }
    }
}