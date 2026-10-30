using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace StockBebidas.Logica
{
    public class GestorStock
    {
        private readonly string rutaArchivo =
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "StockBebidas",
                "bebidas.xml");

        public List<Bebida> Bebidas { get; private set; }

        public GestorStock()
        {
            Bebidas = new List<Bebida>();
            Cargar();
        }

        public void Agregar(Bebida bebida)
        {
            if (bebida == null)
                throw new ArgumentNullException("bebida");

            if (string.IsNullOrWhiteSpace(bebida.Nombre))
                throw new Exception("Ingresá el nombre.");

            if (bebida.Cantidad < 0 || bebida.Precio < 0)
                throw new Exception("Los valores no pueden ser negativos.");

            Bebida existente = Bebidas.Find(
                b => b.Nombre.Equals(
                    bebida.Nombre.Trim(),
                    StringComparison.OrdinalIgnoreCase));

            if (existente != null)
            {
                existente.Cantidad += bebida.Cantidad;
                existente.Precio = bebida.Precio;
            }
            else
            {
                bebida.Nombre = bebida.Nombre.Trim();
                Bebidas.Add(bebida);
            }

            Guardar();
        }

        public void ActualizarStock(int indice, int cantidad)
        {
            if (indice < 0 || indice >= Bebidas.Count)
                throw new Exception("Seleccioná una bebida válida.");

            if (cantidad < 0)
                throw new Exception("Cantidad inválida.");

            Bebidas[indice].Cantidad = cantidad;
            Guardar();
        }

        public void Guardar()
        {
            string carpeta = Path.GetDirectoryName(rutaArchivo);
            Directory.CreateDirectory(carpeta);

            XmlSerializer serializador =
                new XmlSerializer(typeof(List<Bebida>));

            using (FileStream archivo = File.Create(rutaArchivo))
            {
                serializador.Serialize(archivo, Bebidas);
            }
        }

        private void Cargar()
        {
            if (!File.Exists(rutaArchivo))
                return;

            XmlSerializer serializador =
                new XmlSerializer(typeof(List<Bebida>));

            using (FileStream archivo = File.OpenRead(rutaArchivo))
            {
                Bebidas = (List<Bebida>)
                    serializador.Deserialize(archivo);
            }
        }
    }
}