using System;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using postcard.Models;
using Microsoft.Data.SqlClient;

namespace postcard.Controllers
{
	public class SQLUtility
	{
		public List<Capas> getcapas(string query, IConfiguration configuration)
        {
            var capas = new List<Capas>();

			string connectionString = configuration.GetValue<string>("ConnectionStrings:SQLDatabase");

			try
			{
				using (SqlConnection con = new SqlConnection(connectionString))
				{
					con.Open();
					SqlCommand cmd = new SqlCommand(query);
					cmd.Connection = con;

					SqlDataReader rdr = cmd.ExecuteReader();
					while (rdr.Read())
					{
						Capas capa = new Capas();

						capa.id = Convert.ToInt32(rdr["id"]);
                        capa.uf = rdr["uf"].ToString();
                        capa.musica = rdr["musica"].ToString();
                        capa.youtube = rdr["youtube"].ToString();
                        capa.imagem = rdr["imagem"].ToString();

						capas.Add(capa);
                    }
				}
			}
			catch (SqlException ex)
			{
				throw ex;
            }
            return (capas);
		}
	}
}

