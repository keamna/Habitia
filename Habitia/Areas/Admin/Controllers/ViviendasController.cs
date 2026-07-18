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
                Id = v.Id,

                Numero = v.Numero,

                Tipo = v.Tipo,

                Estado = v.Estado,

                CantidadInquilinos = v.CantidadInquilinos,


                PropietarioNombre =
                    v.Usuarios
                    .Where(x =>
                        x.TipoRelacion == TipoRelacionEnum.Propietario)
                    .Select(x =>
                        x.Usuario.Nombre + " " + x.Usuario.Apellido)
                    .FirstOrDefault()
                    ?? "Sin propietario",


                ViveAhi =
                    v.Usuarios
                    .Where(x =>
                        x.TipoRelacion == TipoRelacionEnum.Propietario)
                    .Select(x => x.ViveAhi)
                    .FirstOrDefault(),


                InquilinosActuales =
                    v.Usuarios
                    .Count(x =>
                        x.TipoRelacion == TipoRelacionEnum.Inquilino &&
                        x.Estado == EstadoUsuarioEnum.Activo)

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
                Numero = model.Numero,

                Tipo = model.Tipo,

                CantidadInquilinos = model.CantidadInquilinos,

                Estado = EstadoViviendaEnum.Disponible
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
                    v.Tipo == tipo &&
                    v.Estado == EstadoViviendaEnum.Disponible)
                .ToListAsync();


            var resultado = new List<object>();


            foreach (var vivienda in viviendas)
            {
                var propietario =
                    vivienda.Usuarios
                    .FirstOrDefault(x =>
                        x.TipoRelacion == TipoRelacionEnum.Propietario &&
                        x.Estado == EstadoUsuarioEnum.Activo);



                var inquilinos =
                    vivienda.Usuarios
                    .Count(x =>
                        x.TipoRelacion == TipoRelacionEnum.Inquilino &&
                        x.Estado == EstadoUsuarioEnum.Activo);



                bool disponible = false;



                if (propietario == null)
                {
                    disponible = true;
                }
                else if (
                    propietario.ViveAhi == false &&
                    inquilinos < vivienda.CantidadInquilinos)
                {
                    disponible = true;
                }



                if (disponible)
                {
                    resultado.Add(new
                    {
                        id = vivienda.Id,
                        numero = vivienda.Numero
                    });
                }
            }


            return Json(resultado);
        }
    }
}