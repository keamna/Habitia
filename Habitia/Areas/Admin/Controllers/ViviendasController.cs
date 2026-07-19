using Habitia.Data;
using Habitia.Enums;
using Habitia.Models;
using Habitia.ViewModels.Vivienda;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ViviendasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ViviendasController(ApplicationDbContext context)
        {
            _context = context;
        }


        // LISTADO DE VIVIENDAS

        public async Task<IActionResult> Index()
        {
            var viviendas = await _context.Viviendas
                .Include(v => v.Usuarios)
                .ThenInclude(vu => vu.Usuario)
                .ToListAsync();


            var lista = viviendas.Select(v => new ViviendaListaViewModel
            {
                Id = v.TN_Id,

                Numero = v.TC_Numero,

                Tipo = v.TN_Tipo,
                Estado = v.TN_Estado,

                CantidadInquilinos = v.TN_CantidadInquilinos,


                PropietarioNombre =
                    v.Usuarios
                    .Where(x =>
                        x.TN_TipoRelacion == TipoRelacionEnum.Propietario)
                    .Select(x =>
                        x.Usuario.TC_Nombre + " " + x.Usuario.TC_Apellido)
                    .FirstOrDefault()
                    ?? "Sin propietario",


                ViveAhi =
                    v.Usuarios
                    .Where(x =>
                        x.TN_TipoRelacion == TipoRelacionEnum.Propietario)
                    .Select(x => x.TB_ViveAhi)
                    .FirstOrDefault(),


                InquilinosActuales =
                    v.Usuarios
                    .Count(x =>
                        x.TN_TipoRelacion == TipoRelacionEnum.Inquilino &&
                        x.TN_Estado == EstadoUsuarioEnum.Activo)

            }).ToList();


            return View(lista);
        }



        // GET CREAR VIVIENDA

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }



        // POST CREAR VIVIENDA

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(ViviendaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var vivienda = new Vivienda
            {
                TC_Numero = model.Numero,

                TN_Tipo = model.Tipo,

                TN_CantidadInquilinos = model.CantidadInquilinos,
                TN_Estado = EstadoViviendaEnum.Disponible
            };


            _context.Viviendas.Add(vivienda);

            await _context.SaveChangesAsync();


            TempData["MensajeExito"] = "Vivienda creada correctamente.";


            return RedirectToAction(nameof(Index));
        }



        // OBTENER VIVIENDAS DISPONIBLES PARA REGISTRO

        [HttpGet]
        public async Task<IActionResult> Disponibles(
            TipoViviendaEnum tipo)
        {
            var viviendas = await _context.Viviendas
                .Include(v => v.Usuarios)
                .Where(v =>
                    v.TN_Tipo == tipo &&
                    v.TN_Estado == EstadoViviendaEnum.Disponible)
                .ToListAsync();


            var resultado = new List<object>();


            foreach (var vivienda in viviendas)
            {
                var propietario =
                    vivienda.Usuarios
                    .FirstOrDefault(x =>
                        x.TN_TipoRelacion == TipoRelacionEnum.Propietario &&
                        x.TN_Estado == EstadoUsuarioEnum.Activo);



                var inquilinos =
                    vivienda.Usuarios
                    .Count(x =>
                        x.TN_TipoRelacion == TipoRelacionEnum.Inquilino &&
                        x.TN_Estado == EstadoUsuarioEnum.Activo);



                bool disponible = false;



                if (propietario == null)
                {
                    disponible = true;
                }
                else if (
                    propietario.TB_ViveAhi == false &&
                    inquilinos < vivienda.TN_CantidadInquilinos)
                {
                    disponible = true;
                }



                if (disponible)
                {
                    resultado.Add(new
                    {
                        id = vivienda.TN_Id,
                        numero = vivienda.TC_Numero
                    });
                }
            }


            return Json(resultado);
        }
    }
}