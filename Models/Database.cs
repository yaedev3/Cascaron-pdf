using System.Data.SqlClient;
using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using ApiSIES.Models;
using Microsoft.AspNetCore.Mvc;
using iTextSharp.text.pdf.qrcode;
using Org.BouncyCastle.Asn1.Ocsp;

namespace ApiSIES.Models
{
    public class Database
    {
        //para beta
        private static string conexion= "data source=148.224.138.50;initial catalog=SiesEscolar;user id=SiesEscolar;password=Kuka!02;database=SIIA";
        
        //para produccion
        //private static string conexion = "data source=148.224.245.75;initial catalog=SiesEscolar;user id=SiesEscolar;password=Kuka!02;database=SIIA";
        
        private static string conexionEscolares = "data source=148.224.140.100,2326;initial catalog=sies;user id=sies;password=13793$col@r.8;database=Escolares";
        private string Facultad = "";
        private string ProgramaEducativo = "";

        public void asigna_facultad(string cadena)
        {
            Facultad = cadena;
            
        }
        public void asigna_programaeducativo(string cadena)
        {
            ProgramaEducativo = cadena;

        }
        public string dame_facultad()
        {
            return Facultad;

        }
        public string dame_programaeducativo()
        {
            return ProgramaEducativo;

        }
        public IEnumerable<Alumno> Api(int claveunica,ref List<Materia> l, ref List<Materia> lista_ingles, ref EstadisticaAlumno estadistica)
        {
           
            DataTable dtEsc = new DataTable();
            DataTable dtMateriasinscritas = new DataTable();
            DataTable dtDatos = new DataTable();
            //tabla para Kardex Pantalla
            DataTable tab = new DataTable();
            //tabla para estadisticas
            DataTable tab2 = new DataTable();
            byte[] foto = null;
            string f;
            bool esRegular=true;
            bool materiasinscritas=true;
            string ClaveCarrera = " ";
            List<Materia> list = l;
            DataColumn dc = new DataColumn("ClaveUnica", typeof(int));
            dtDatos.Columns.Add(dc);
            dc = new DataColumn("Semestre", typeof(int));
            dtDatos.Columns.Add(dc);
            dc = new DataColumn("Activo", typeof(bool));
            dtDatos.Columns.Add(dc);
            dc = new DataColumn("IdPlan", typeof(int));
            dtDatos.Columns.Add(dc);
            dc = new DataColumn("Generacion", typeof(int));
            dtDatos.Columns.Add(dc);
            dc = new DataColumn("Regular", typeof(bool));
            dtDatos.Columns.Add(dc);
            dc = new DataColumn("ProgramaEducativo", typeof(string));
            dtDatos.Columns.Add(dc);
            dc = new DataColumn("Facultad", typeof(string));
            dtDatos.Columns.Add(dc);
            dc = new DataColumn("paterno", typeof(string));
            dtDatos.Columns.Add(dc);
            dc = new DataColumn("materno", typeof(string));
            dtDatos.Columns.Add(dc);
            dc = new DataColumn("nombre", typeof(string));
            dtDatos.Columns.Add(dc);
            dc = new DataColumn("sexo", typeof(string));
            dtDatos.Columns.Add(dc);
            dc = new DataColumn("curp", typeof(string));
            dtDatos.Columns.Add(dc);
            dc = new DataColumn("tutor", typeof(string));
            dtDatos.Columns.Add(dc);
            dc = new DataColumn("IdDES", typeof(int));
            dtDatos.Columns.Add(dc);
            dc = new DataColumn("calle", typeof(string));
            dtDatos.Columns.Add(dc);
            dc = new DataColumn("numext", typeof(string));
            dtDatos.Columns.Add(dc);
            dc = new DataColumn("colonia", typeof(string));
            dtDatos.Columns.Add(dc);
            dc = new DataColumn("cp", typeof(string));
            dtDatos.Columns.Add(dc);
            dc = new DataColumn("telefono", typeof(string));
            dtDatos.Columns.Add(dc);
            dc = new DataColumn("celular", typeof(string));
            dtDatos.Columns.Add(dc);
            dc = new DataColumn("correopersonal", typeof(string));
            dtDatos.Columns.Add(dc);
            dc = new DataColumn("correoinsitucional", typeof(string));
            dtDatos.Columns.Add(dc);
            dc = new DataColumn("cidcarrera", typeof(string));
            dtDatos.Columns.Add(dc);

            try
            {
                    SqlConnection conexionEsc = new SqlConnection(conexionEscolares);
                    SqlConnection conexionExp = new SqlConnection(conexion);
                    SqlDataAdapter sdaEscolares;

                    conexionEsc.Open();
                    conexionExp.Open();


                    string consultaEsc = $"SELECT paterno,materno,nombre,sexo,curp,tutor,calle,numext,colonia,cp,telefono,celular,correopersonal,correoinstitucional,cidcarrera FROM dbo.SIESConsultaAlumnoClaveUnica({claveunica})";
                    sdaEscolares = new SqlDataAdapter(consultaEsc, conexionEsc);
                    sdaEscolares.Fill(dtEsc);

                    string consultaMat = $"SELECT top(1) * FROM [SIIA].[SIES].[EstadosAlumno] where ClaveUnica = {claveunica} order by Fecha desc";
                    sdaEscolares = new SqlDataAdapter(consultaMat, conexionExp);
                    sdaEscolares.Fill(dtMateriasinscritas);


                    string consultaExp = "select ClaveUnica ,Semestre,Activo,p.IdPlan, Generacion, Regular, ProgramaEducativo, d2.Nombre,p2.IdDES from ((SIIA.SIES.Din2 as d join SIIA.SIES.Planes as p on d.IdPlan=p.IdPlan) join SIIA.ServiciosEscolares.PE as p2 on p2.IdEscolares = p.IdCarrera)join SIIA.ServiciosEscolares.Des as d2 on d2.IdDes = p2.IdDES   where ClaveUnica =" + claveunica;
                    SqlCommand scc = new SqlCommand(consultaExp, conexionExp);
                    SqlDataReader drDat = scc.ExecuteReader();

                    //consulta para sacar la fotografia
                    string consultaFoto = "SELECT fotografia FROM dbo.SIESConsultaAlumnoClaveUnica(" + claveunica + ")";
                    SqlCommand scComand = new SqlCommand(consultaFoto, conexionEsc);
                    foto = (byte[])scComand.ExecuteScalar();
                    if (foto != null)
                    {
                        f = Convert.ToBase64String(foto);
                    }
                    else
                    {
                        f = "";
                    }
                    
                //NUEVO a integrar
                int idplan = 0;
                if (drDat.HasRows)
                    {
                        drDat.Read();
                        DataRow dr = dtDatos.NewRow();
                        dr[0] = Convert.ToInt32(drDat[0]);//claveUnica
                        dr[1] = Convert.ToInt32(drDat[1]);//Semestre
                        dr[2] = Convert.ToBoolean(drDat[2]);//Activo
                        dr[3] = Convert.ToInt32(drDat[3]);//IdPlan
                        idplan = Convert.ToInt32(drDat[3]);
                        dr[4] = Convert.ToInt32(drDat[4]);//Generacion
                        dr[5] = Convert.ToBoolean(drDat[5]);//Regular
                        dr[6] = drDat[6].ToString();//ProgramaEducativo
                        dr[7] = drDat[7].ToString();//facultad
                        asigna_programaeducativo(drDat[6].ToString());
                        asigna_facultad(drDat[7].ToString());
                        dr[8] = dtEsc.Rows[0][0].ToString().Trim();//paterno
                        dr[9] = dtEsc.Rows[0][1].ToString().Trim();//materno
                        dr[10] = dtEsc.Rows[0][2].ToString().Trim();//nombre
                        dr[11] = Convert.ToChar(dtEsc.Rows[0][3]);//sexo
                        dr[12] = dtEsc.Rows[0][4].ToString();//curp
                        dr[13] = dtEsc.Rows[0][5].ToString().Trim();//tutor
                        dr[14] = Convert.ToInt32(drDat[8]);//idDES
                        dr[15] = dtEsc.Rows[0][6].ToString().Trim();//calle
                        dr[16] = dtEsc.Rows[0][7].ToString().Trim();//numext
                        dr[17] = dtEsc.Rows[0][8].ToString().Trim();//colonia
                        dr[18] = dtEsc.Rows[0][9].ToString().Trim();//cp
                        dr[19] = dtEsc.Rows[0][10].ToString().Trim();//telefono
                        dr[20] = dtEsc.Rows[0][11].ToString().Trim();//celular
                        dr[21] = dtEsc.Rows[0][12].ToString().Trim();//correopersonal
                        dr[22] = dtEsc.Rows[0][13].ToString().Trim();//correoinstitucional
                        dr[23] = dtEsc.Rows[0][14].ToString().Trim();//cidcarrera

                            //Aqui Comienza lo del Kardex
                            SqlConnection EscConnection = new SqlConnection(conexion);
                            SqlCommand cmd = new SqlCommand();
                            cmd.CommandText = "SIES.KardexImpresionAlumnoA";
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add("@ClaveUnica", SqlDbType.Int);
                            cmd.Parameters.Add("@IdPlan", SqlDbType.Int);
                            cmd.Parameters.Add("@promSem", SqlDbType.Bit);


                            cmd.Parameters["@ClaveUnica"].Value = claveunica;
                            cmd.Parameters["@IdPlan"].Value = idplan;
                            cmd.Parameters["@promSem"].Value = false;

                            cmd.Connection = EscConnection;

                            EscConnection.Open();
                            var algo = cmd.ExecuteNonQuery();
                            SqlDataAdapter nuevo = new SqlDataAdapter(cmd);
                            nuevo.Fill(tab);
                            EscConnection.Close();

                            //eSTADISTICA
                            EscConnection = new SqlConnection(conexion);
                            cmd = new SqlCommand();
                            cmd.CommandText = "SIES.KardexEstadisticaAlumno";
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add("@ClaveUnica", SqlDbType.Int);
                            cmd.Parameters.Add("@IdPlan", SqlDbType.Int);
                            cmd.Parameters.Add("@TotalCreditos", SqlDbType.Bit);

                            cmd.Parameters["@ClaveUnica"].Value = claveunica;
                            cmd.Parameters["@IdPlan"].Value = idplan;
                            cmd.Parameters["@TotalCreditos"].Value = false;

                            cmd.Connection = EscConnection;

                            EscConnection.Open();
                            var resultado = cmd.ExecuteNonQuery();
                            SqlDataAdapter nuevo2 = new SqlDataAdapter(cmd);
                            nuevo2.Fill(tab2);
                            EscConnection.Close();


                            foreach (DataRow row in tab.Rows)
                            {
                                Materia materia = new Materia();
                                materia.IdDetalleMateria = Convert.ToInt32(row["IdDetalleMateria"].ToString());
                                materia.IdTipoDetalleGrupo = Convert.ToInt32(row["IdTipoDetalleGrupo"].ToString());
                                materia.Creditos = Convert.ToInt32(row["Creditos"].ToString());
                                materia.Nombre = row["Nombre"].ToString();
                                materia.IdTipoExamen = row["IdTipoExamen"].ToString();    //IdTipoExamen
                                materia.Calificacion = row["Calificacion"].ToString();//Calificacion
                                materia.Fecha = Convert.ToDateTime(row["Fecha"].ToString());//TipoDetalleGrupo
                                materia.Acta= row["Acta"].ToString();
                                materia.EO1 = row["EO1"].ToString();
                                materia.EE1 = row["EE1"].ToString();
                                materia.ET1 = row["ET1"].ToString();
                                materia.EO2 = row["EO2"].ToString();
                                materia.EE2 = row["EE2"].ToString();
                                materia.ET2 = row["ET2"].ToString();
                                materia.EO3 = row["EO3"].ToString();
                                materia.EE3 = row["EE3"].ToString();
                                materia.ET3 = row["ET3"].ToString();
                                materia.ER1 = row["ER1"].ToString();
                                materia.ER2 = row["ER2"].ToString();
                                materia.ER3 = row["ER3"].ToString();

                                //Checa si la materia es Ingles
                                if (materia.IdTipoDetalleGrupo == 5)
                                {
                                    lista_ingles.Add(materia);
                                }
                                else 
                                {
                                    list.Add(materia);
                                }
                            
                            }
                            foreach (DataRow row in tab2.Rows)
                            {
                                estadistica.Plan=row["Plan"].ToString();
                                estadistica.PromedioGeneral=row["PromedioGeneral"].ToString();
                                estadistica.Aprobadas= row["Aprobadas"].ToString();
                                estadistica.Reprobadas = row["Reprobadas"].ToString();
                                estadistica.Obligatorias = row["Obligatorias"].ToString();
                                estadistica.Optativas = row["Optativas"].ToString();
                                estadistica.Selectivas = row["Selectivas"].ToString();
                                estadistica.EstanciasClinicas = row["EstanciasClinicas"].ToString();
                                estadistica.Regular = row["Regular"].ToString();
                                if (estadistica.Regular == "NO")
                                {
                                    esRegular = false;
                                }
                                else {
                                    esRegular = true;
                                }
                                ClaveCarrera = row["ClaveCarrera"].ToString();

                    }

                        
               
                    dtDatos.Rows.Add(dr);
                        drDat.Close();
                    }
                    else
                    {
                        drDat.Close();
                    }
                


                conexionEsc.Close();
                    conexionExp.Close();
                }
                catch (SqlException error)
                {
                    throw new ArgumentException("Error en la Base de Datos", error);
                }


            var idC = dtDatos.AsEnumerable().Select(rw =>
                        new Alumno
                        {
                            ClaveUnica = Convert.ToInt32(rw["ClaveUnica"]),
                            Semestre = Convert.ToInt32(rw["Semestre"]),
                            Generacion = Convert.ToInt32(rw["Generacion"]),
                            NombreCompleto = rw["paterno"].ToString() +" "+ rw["materno"].ToString() +" "+ rw["nombre"].ToString(),
                            IdDES = Convert.ToInt32(rw["IdDES"]),
                            IdCarrera = Convert.ToInt32(ClaveCarrera),
                            fotografia = f,
                            MateriasIncritas = materiasinscritas,
                            Regular = esRegular,
                        });
     
            return idC;

        }

        

        

       
    }

}
