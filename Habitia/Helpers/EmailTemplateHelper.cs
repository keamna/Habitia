using System;
namespace Habitia.Helpers
{
    public static class EmailTemplateHelper
    {
        public static string GenerarCorreoCodigoVerificacion(string nombreUsuario, string codigo)
        {
            return $@"
            <div style=""font-family: 'DM Sans', Arial, sans-serif; background-color: #F4FFF5; padding: 40px 0;"">
                <div style=""max-width: 480px; margin: 0 auto; background-color: #FFFFFF; border-radius: 12px; overflow: hidden; border: 1px solid #ECECEC;"">
                    <div style=""background-color: #336332; padding: 24px 32px;"">
                        <span style=""font-family: 'Poppins', Arial, sans-serif; font-weight: 700; font-size: 22px; color: #FFFFFF;"">Habitia</span>
                    </div>
                    <div style=""padding: 32px;"">
                        <p style=""font-size: 15px; color: #1a1a1a; margin: 0 0 8px;"">Hola, {nombreUsuario}.</p>
                        <p style=""font-size: 14px; color: #848484; margin: 0 0 24px; line-height: 1.5;"">
                            Recibimos un intento de inicio de sesión en tu cuenta de Habitia. Usá el siguiente código para verificar tu identidad:
                        </p>
                        <div style=""background-color: #F4FFF5; border: 1px solid #ECECEC; border-radius: 9px; padding: 18px; text-align: center; margin-bottom: 24px;"">
                            <span style=""font-family: 'Poppins', Arial, sans-serif; font-size: 28px; font-weight: 700; color: #336332; letter-spacing: 4px;"">{codigo}</span>
                        </div>
                        <p style=""font-size: 13px; color: #848484; margin: 0; line-height: 1.5;"">
                            Este código vence en 10 minutos y solo puede usarse una vez. Si no fuiste vos quien intentó iniciar sesión, ignorá este correo y te recomendamos cambiar tu contraseña.
                        </p>
                    </div>
                    <div style=""background-color: #F4FFF5; padding: 16px 32px; border-top: 1px solid #ECECEC;"">
                        <span style=""font-size: 12px; color: #848484;"">© {DateTime.Now.Year} Habitia. Todos los derechos reservados.</span>
                    </div>
                </div>
            </div>";
        }

        public static string GenerarCorreoContrasenaTemporal(string nombreUsuario, string contrasenaTemporal)
        {
            return $@"
            <div style=""font-family: 'DM Sans', Arial, sans-serif; background-color: #F4FFF5; padding: 40px 0;"">
                <div style=""max-width: 480px; margin: 0 auto; background-color: #FFFFFF; border-radius: 12px; overflow: hidden; border: 1px solid #ECECEC;"">
                    <div style=""background-color: #336332; padding: 24px 32px;"">
                        <span style=""font-family: 'Poppins', Arial, sans-serif; font-weight: 700; font-size: 22px; color: #FFFFFF;"">Habitia</span>
                    </div>
                    <div style=""padding: 32px;"">
                        <p style=""font-size: 15px; color: #1a1a1a; margin: 0 0 8px;"">Hola, {nombreUsuario}.</p>
                        <p style=""font-size: 14px; color: #848484; margin: 0 0 24px; line-height: 1.5;"">
                            Recibimos una solicitud para recuperar tu contraseña en Habitia. Usá la siguiente contraseña temporal para acceder a tu cuenta:
                        </p>
                        <div style=""background-color: #F4FFF5; border: 1px solid #ECECEC; border-radius: 9px; padding: 18px; text-align: center; margin-bottom: 24px;"">
                            <span style=""font-family: 'Poppins', Arial, sans-serif; font-size: 20px; font-weight: 700; color: #336332; letter-spacing: 2px;"">{contrasenaTemporal}</span>
                        </div>
                        <p style=""font-size: 13px; color: #848484; margin: 0 0 16px; line-height: 1.5;"">
                            <strong>Pasos a seguir:</strong><br/>
                            1. Inicia sesión con esta contraseña temporal<br/>
                            2. Ve a Mi Perfil<br/>
                            3. Cambia tu contraseña a una nueva segura
                        </p>
                        <p style=""font-size: 13px; color: #E53935; margin: 0; line-height: 1.5;"">
                            <strong>⚠️ Esta contraseña expira en 24 horas.</strong> Si no solicitaste este cambio, puedes ignorar este correo.
                        </p>
                    </div>
                    <div style=""background-color: #F4FFF5; padding: 16px 32px; border-top: 1px solid #ECECEC;"">
                        <span style=""font-size: 12px; color: #848484;"">© {DateTime.Now.Year} Habitia. Todos los derechos reservados.</span>
                    </div>
                </div>
            </div>";
        }
    }
}