using ApiSIES.Models;
using Grpc.Core;
using iText.IO.Image;
using iText.Kernel.Events;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Layout;
using iText.Layout.Renderer;
using iText.Layout.Properties;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.IO.Font;
using iText.Layout.Borders;
using iTextSharp.text.pdf;
using PdfWriter = iText.Kernel.Pdf.PdfWriter;
using PdfDocument = iText.Kernel.Pdf.PdfDocument;
using PdfPage = iText.Kernel.Pdf.PdfPage;
using PdfFont = iText.Kernel.Font.PdfFont;
using Org.BouncyCastle.Utilities;
using iTextSharp.text;
using Paragraph = iText.Layout.Element.Paragraph;
using Document = iText.Layout.Document;
using Image = iText.Layout.Element.Image;
using PageSize = iText.Kernel.Geom.PageSize;
using Rectangle = iText.Kernel.Geom.Rectangle;
using System.Drawing;
using iText.StyledXmlParser.Jsoup.Parser;
using iText.Pdfa;

namespace ApiSIES.Controllers
{
    public class PdfDownloadController : Controller
    {

        [Route("Api/Kardex/Respaldo/{id:int}")]
        public ActionResult Pdf(int id=0)
        {
            //aqui se llama los datos del usuario
            Database dt = new Database();
            List<Materia> lista_ingles = new List<Materia>();
            List<Materia> list = new List<Materia>();
            EstadisticaAlumno estadistica = new EstadisticaAlumno();

            //var query = dt.Api(id, ref list, ref lista_ingles, ref estadistica);
            string Facultad = dt.dame_facultad();
            string ProgramaEducativo = dt.dame_programaeducativo();
            MemoryStream ms = new MemoryStream();
            PdfWriter pw = new PdfWriter(ms);
            PdfDocument pdfDocument = new PdfDocument(pw);
            Document doc = new Document(pdfDocument, PageSize.LETTER);
            PdfFont font;
            Paragraph para;
            int num = 1;
            bool InglesconCreditos = true;
            float[] anchosDui;
            string[] titulosDui;
            Cell cell;
            DeviceRgb color;



                // Aqui se dibuja el logo de la UASLP
                String imageFile = "Imagenes/logo.png";
                ImageData data = ImageDataFactory.Create(imageFile);
                // Creating an Image object 
                Image logo = new Image(data);
                // Adding image to the document 
                logo.SetWidth(30);
                //doc.Add(img);

                //logo entidad
                imageFile = "Imagenes/logo.png";
                data = ImageDataFactory.Create(imageFile);
                Image logo2 = new Image(data);
                logo2.SetWidth(30);
                //doc.Add(img);


                Table table = new Table(new float[] { .1f, .3f, .1f }).UseAllAvailableWidth().SetWidth(500);

                //dibuja el encabezado
                for (int i = 0; i < 4; i++)
                {
                    cell = new Cell();
                    //cell.SetMinHeight(50);
                    cell.SetVerticalAlignment(VerticalAlignment.MIDDLE);
                    cell.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                    cell.SetTextAlignment(TextAlignment.CENTER);
                    cell.SetBorder(Border.NO_BORDER);


                    if (i == 0)
                    {
                        logo.SetTextAlignment(TextAlignment.CENTER);
                        cell.Add(logo);
                        table.AddCell(cell);
                    }
                    else if (i == 1)
                    {
                        font = PdfFontFactory.CreateFont("Letras/arial.ttf");
                        //Encabecado de la Universidad
                        para = new Paragraph("UNIVERSIDAD AUTÓNOMA DE SAN LUIS POTOSÍ").SetFont(font).SetBold().SetFontSize(12).SetFontColor(ColorConstants.GRAY).Add("\n");
                        //Encabezado de la Facultad
                        para.Add(new Paragraph(Facultad).SetFont(font).SetFontSize(10).SetFontColor(ColorConstants.GRAY).Add("\n").Add(new Paragraph("KARDEX GENERAL DEL ALUMNO").SetFont(font).SetFontSize(8).SetFontColor(ColorConstants.GRAY).Add("\n")));
                        font = PdfFontFactory.CreateFont("Letras/Calibri.ttf");
                        //Kardex General
                        //para);
                        cell.Add(para);
                        table.AddCell(cell);
                    }
                    else if (i == 2)
                    {
                        cell.Add(logo2);
                        table.AddCell(cell);
                    }


                }
                table.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                doc.Add(table);
                SolidLine line = new SolidLine(0.5f);
                line.SetColor(ColorConstants.BLACK);
                LineSeparator ls = new LineSeparator(line);
                ls.SetWidth(535);
                ls.SetMarginTop(5);
                doc.Add(ls);

                //dibuja fecha de hoy
                PdfFont fuente;
                Paragraph parrafo;
                // Display the date in the default (general) format.
                fuente = PdfFontFactory.CreateFont("Letras/Calibri.ttf");
                parrafo = new Paragraph(DateTime.Now.ToString()).SetFont(fuente).SetFontSize(8).SetFontColor(ColorConstants.BLACK).SetTextAlignment(TextAlignment.RIGHT);
                doc.Add(parrafo);


                //dibuja Foto y materias
                Table tabla_datosalumno = new Table(new float[] { .17f, .17f, .16f, .50f }).UseAllAvailableWidth().SetWidth(450);

                for (int i = 0; i < 5; i++)
                {
                    cell = new Cell();
                    //cell.SetMinHeight(50);
                    cell.SetVerticalAlignment(VerticalAlignment.MIDDLE);
                    cell.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                    cell.SetTextAlignment(TextAlignment.CENTER);
                    cell.SetBorder(Border.NO_BORDER);


                    if (i == 0)
                    {
                        //espacio en blanco
                        font = PdfFontFactory.CreateFont("Letras/Calibri.ttf");
                        para = new Paragraph(" ").SetFont(font).SetBold().SetFontSize(8).SetFontColor(ColorConstants.BLACK);
                        cell.Add(para);
                        tabla_datosalumno.AddCell(cell);
                    }
                    else if (i == 1)
                    {
                        //fotografia
                        imageFile = "Imagenes/logo.png";
                        data = ImageDataFactory.Create(imageFile);
                        Image image = new Image(data);
                        image.SetWidth(65f);
                        cell.Add(image);
                        tabla_datosalumno.AddCell(cell);
                    }
                    else if (i == 2)
                    {
                        cell.SetTextAlignment(TextAlignment.LEFT);
                        font = PdfFontFactory.CreateFont("Letras/Calibri.ttf");
                        //Encabecado de la Universidad
                        para = new Paragraph("Clave Única: \n Nombre: \n Carrera: \n Generación: \n Plan de Estudios: \n Promedio General: \n").SetFont(font).SetBold().SetFontSize(8).SetFontColor(ColorConstants.BLACK);
                        cell.Add(para);
                        tabla_datosalumno.AddCell(cell);
                    }
                    else if (i == 3)
                    {
                        cell.SetTextAlignment(TextAlignment.LEFT);
                        font = PdfFontFactory.CreateFont("Letras/Calibri.ttf");
                        //Encabecado de la Universidad
                        para = new Paragraph("000000" + "\n" + "Rodrigo Messi" + "\n" +"Ingeniero" + "\n" + "2010" + "\n" +"2015"+ "\n" + "8.0" + "\n").SetFont(font).SetFontSize(8).SetFontColor(ColorConstants.BLACK);
                        cell.Add(para);
                        tabla_datosalumno.AddCell(cell);
                    }

                }
                tabla_datosalumno.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                doc.Add(tabla_datosalumno);

                float[] anchos;
                string[] titulos;
                List<float> widths = new List<float>();
                List<string> titles = new List<string>();

                bool eo1 = false;
                
                Materia materia = new Materia();
                materia.Creditos =2;
                materia.Acta = "20 febreo 2012";
                materia.Nombre = "Conquista el Mundo 1";
                materia.EO1 = "8.0";
                list.Add(materia);
                list.Add(materia);
                list.Add(materia);

                foreach (Materia mat in list)
                {
                    if (mat.EO1 != String.Empty && eo1 == false)
                    {
                        eo1 = true;
                    }
                }

                widths.Add(.04f);                       //No.
                titles.Add("No.");
                widths.Add(.07f);                       //Clave
                titles.Add("CLAVE");
                widths.Add(.45f);                       //Materia
                titles.Add(" MATERIA");
                if (eo1)
                {
                    widths.Add(.03f);                       //EO Primera Inscripción
                    titles.Add("EO");
                }
               
                widths.Add(.06f);                       //Acta
                titles.Add("ACTA");
                widths.Add(.08f);                       //Fecha
                titles.Add("FECHA");

                anchos = widths.ToArray();
                titulos = titles.ToArray();

                Table tabla_materias = new Table(anchos).UseAllAvailableWidth();
                for (int i = 0; i < titulos.Length; i++)
                {
                    cell = new Cell();
                    cell.SetVerticalAlignment(VerticalAlignment.MIDDLE);
                    cell.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                    cell.SetTextAlignment(TextAlignment.CENTER);
                    font = PdfFontFactory.CreateFont("Letras/Calibri.ttf");
                    para = new Paragraph(titulos[i]).SetFont(font).SetBold().SetFontSize(7).SetFontColor(ColorConstants.BLACK);
                    cell.Add(para);
                    tabla_materias.AddCell(cell);

                }

                num = 1;
                foreach (Materia m in list)
                {

                    if (num % 2 == 0)
                    {
                        //color Gris
                        color = new DeviceRgb(192, 192, 192);
                    }
                    else
                    {
                        //color blanco
                        color = new DeviceRgb(255, 255, 255);
                    }

                    font = PdfFontFactory.CreateFont("Letras/Calibri.ttf");
                    //num
                    imprime(num.ToString(), font, 7, TextAlignment.CENTER, color, Border.NO_BORDER, ref tabla_materias);
                    //Clave
                    imprime(m.IdDetalleMateria.ToString(), font, 7, TextAlignment.CENTER, color, Border.NO_BORDER, ref tabla_materias);
                    //Materia
                    imprime(m.Nombre.ToString(), font, 7, TextAlignment.LEFT, color, Border.NO_BORDER, ref tabla_materias);
                    if (eo1)
                    {
                        //EO1
                        imprime(m.EO1.ToString(), font, 7, TextAlignment.CENTER, color, Border.NO_BORDER, ref tabla_materias);
                    }
                    //ACTA
                    imprime(m.Acta.ToString(), font, 7, TextAlignment.CENTER, color, Border.NO_BORDER, ref tabla_materias);
                    //FECHA
                    imprime(m.Fecha.ToString("dd/MM/yyyy"), font, 7, TextAlignment.CENTER, color, Border.NO_BORDER, ref tabla_materias);
                    num++;
                }
                doc.Add(tabla_materias);
            


            if (InglesconCreditos)
            {
                anchosDui = new float[] { .04f, .07f, .45f, .10f, .10f, .06f, .08f };
                titulosDui = new string[] { "No.", "CLAVE", " MATERIA", "CALIFICACIÓN", "TIPO EXAMEN", "CRED", "FECHA" };
            }
            else
            {
                anchosDui = new float[] { .04f, .07f, .45f, .10f, .10f, .08f };
                titulosDui = new string[] { "No.", "CLAVE", " MATERIA", "CALIFICACIÓN", "TIPO EXAMEN", "FECHA" };
            }


            doc.Close();

            byte[] bytesStream = ms.ToArray();
            ms = new MemoryStream();
            ms.Write(bytesStream, 0, bytesStream.Length);
            ms.Position = 0;

            return new FileStreamResult(ms, "application/pdf");

        }

        void imprime(string imprimir, PdfFont font, int tam, TextAlignment alineacion, DeviceRgb color, Border borde, ref Table tabla)
        {
            Paragraph para = new Paragraph(imprimir.ToString()).SetFont(font).SetFontSize(tam).SetFontColor(ColorConstants.BLACK);
            Cell cell = new Cell();
            cell.SetTextAlignment(alineacion);
            cell.SetBackgroundColor(color);
            cell.SetBorder(borde);
            cell.Add(para);
            tabla.AddCell(cell);
        }
        void imprime_encabezado(string imprimir, PdfFont font, int tam, TextAlignment alineacion, ref Table tabla)
        {
            Cell cell = new Cell();
            cell.SetVerticalAlignment(VerticalAlignment.MIDDLE);
            cell.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            cell.SetTextAlignment(alineacion);
            Paragraph para = new Paragraph(imprimir).SetFont(font).SetBold().SetFontSize(tam).SetFontColor(ColorConstants.BLACK);
            cell.Add(para);
            tabla.AddCell(cell);
        }
    }
}
