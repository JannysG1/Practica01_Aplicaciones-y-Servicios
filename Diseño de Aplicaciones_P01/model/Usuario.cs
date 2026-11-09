namespace Diseño_de_Aplicaciones_P01.model
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string CorreoElectronicousuario { get; set; } = string.Empty;
        public string Contrasenausuario { get; set; } = string.Empty;
        public bool Activo { get; set; }
    }
}
