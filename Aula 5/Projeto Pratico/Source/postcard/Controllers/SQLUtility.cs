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
        public string getconnectionstring(IConfiguration configuration)
        {
#if DEBUG
            var connectionString = configuration.GetValue<string>("ConnectionStrings:SQLDatabaseConnection");
#else
            var connectionString = configuration.GetConnectionString("SQLDatabaseConnection");
#endif
            return connectionString;
        }

		public List<Capas> getcapas(string query, IConfiguration configuration)
        {
            var capas = new List<Capas>();			

			try
			{
				using (SqlConnection con = new SqlConnection(getconnectionstring(configuration)))
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
                        capa.estado = rdr["estado"].ToString();
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

		public List<PostCards> getcards(string query, IConfiguration configuration)
		{
            var cards = new List<PostCards>();

            try
            {
                using (SqlConnection con = new SqlConnection(getconnectionstring(configuration)))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(query);
                    cmd.Connection = con;

                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        PostCards card = new PostCards();

                        card.id = Convert.ToInt32(rdr["id"]);
                        card.id_uf = Convert.ToInt32(rdr["id_uf"]);
                        card.uf = rdr["uf"].ToString();
                        card.cidade = rdr["cidade"].ToString();
                        card.card = rdr["card"].ToString();
                        card.descricao = rdr["descricao"].ToString();

                        cards.Add(card);
                    }
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            return (cards);
        }
	}
}

