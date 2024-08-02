using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MercDevs_ej2.Models;
using Microsoft.CodeAnalysis;
using System;
using Microsoft.AspNetCore.Authorization;

namespace MercDevs_ej2.Controllers
{
    [Authorize]
    public class RecepcionequipoesController : Controller
    {
        private readonly MercyDeveloperContext _context;

        public RecepcionequipoesController(MercyDeveloperContext context)
        {
            _context = context;
        }

        // GET: Recepcionequipoes
        public async Task<IActionResult> Index()
        {
            var mercydevsEjercicio2Context = _context.Recepcionequipos.Include(r => r.IdClienteNavigation).Include(r => r.IdServicioNavigation).OrderBy(r => r.Id);
            return View(await mercydevsEjercicio2Context.ToListAsync());
        }

        // GET: Recepcionequipoes/IndexId
        [HttpGet]
        [Route("Recepcionequipoes/IndexId/{idCliente}")]
        public async Task<IActionResult> IndexId(int idCliente)
        {
            // Obtener las recepciones para el cliente especificado
            var recepciones = _context.Recepcionequipos
                                      .Include(r => r.IdServicioNavigation)
                                      .Include(r => r.IdClienteNavigation)
                                      .Where(r => r.IdCliente == idCliente);

            // Obtener el cliente
            var cliente = await _context.Clientes.FindAsync(idCliente);
            if (cliente == null)
            {
                return NotFound();
            }

            // Pasar el cliente al ViewBag
            ViewBag.ClienteNombre = $"{cliente.Nombre} {cliente.Apellido}";
            ViewBag.IdCliente = idCliente;

            return View(await recepciones.ToListAsync());
        }

        // GET: Recepcionequipoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recepcionequipo = await _context.Recepcionequipos
                .Include(r => r.IdClienteNavigation)
                .Include(r => r.IdServicioNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recepcionequipo == null)
            {
                return NotFound();
            }

            return View(recepcionequipo);
        }

        public async Task<IActionResult> DetailsId(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recepcionequipo = await _context.Recepcionequipos
                .Include(r => r.IdClienteNavigation)
                .Include(r => r.IdServicioNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recepcionequipo == null)
            {
                return NotFound();
            }

            return View(recepcionequipo);
        }

        // GET: Recepcionequipoes/Create
        public IActionResult Create(int? idCliente)
        {
            // Obtener los clientes para el dropdown
            var clientes = _context.Clientes.ToList();

            if (idCliente.HasValue)
            {
                // Si idCliente se proporciona, pasarlo como un valor oculto
                ViewData["IdCliente"] = idCliente.Value;
                ViewData["Clientes"] = new SelectList(clientes, "IdCliente", "IdCliente");
            }
            else
            {
                // Si idCliente no se proporciona, preparar el dropdown
                ViewData["IdCliente"] = new SelectList(clientes, "IdCliente", "IdCliente");
            }

            ViewData["IdServicio"] = new SelectList(_context.Servicios, "IdServicio", "IdServicio");
            ViewData["TipoPc"] = new SelectList(new[]
            {
                new { Value = 0, Text = "PC Torre" },
                new { Value = 1, Text = "Notebook" },
                new { Value = 2, Text = "All-In-ONE" }
            }, "Value", "Text");

            ViewData["CapacidadRam"] = new SelectList(new[]
            {
                new { Value = 0, Text = "4 GB" },
                new { Value = 1, Text = "6 GB" },
                new { Value = 2, Text = "8 GB" },
                new { Value = 3, Text = "12 GB" },
                new { Value = 4, Text = "Otra" }
            }, "Value", "Text");

            ViewData["TipoAlmacenamiento"] = new SelectList(new[]
            {
                new { Value = 0, Text = "HDD" },
                new { Value = 1, Text = "SSD Sata" },
                new { Value = 2, Text = "SSD M.2" },
                new { Value = 3, Text = "SSD NVM M.2" }
            }, "Value", "Text");

            ViewData["TipoGpu"] = new SelectList(new[]
            {
                new { Value = 0, Text = "Chip Integrado" },
                new { Value = 1, Text = "Tarjeta" }
            }, "Value", "Text");

            ViewData["Estado"] = new SelectList(new[]
            {
                new { Value = 1, Text = "Activo" },
                new { Value = 0, Text = "Inactivo" }
            }, "Value", "Text");

            return View();
        }

        // POST: Recepcionequipoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create([Bind("Id,IdCliente,IdServicio,Fecha,TipoPc,Accesorio,MarcaPc,ModeloPc,Nserie,CapacidadRam,TipoAlmacenamiento,CapacidadAlmacenamiento,TipoGpu,Grafico,Estado")] Recepcionequipo recepcionequipo)
        {
            if (recepcionequipo.TipoGpu != null)
            {
                // Si el estado es válido, establece el estado por defecto y guarda en la base de datos
                recepcionequipo.Estado = "1"; // Asegúrate de que el estado sea el correcto
                _context.Add(recepcionequipo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Redirige a la vista de índice o la página deseada
            }

            // Si hay errores, vuelve a mostrar el formulario con los datos ingresados
            ViewData["IdCliente"] = new SelectList(_context.Clientes, "IdCliente", "IdCliente", recepcionequipo.IdCliente);
            ViewData["IdServicio"] = new SelectList(_context.Servicios, "IdServicio", "IdServicio", recepcionequipo.IdServicio);
            return View(recepcionequipo);
        }


        // GET: Recepcionequipoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recepcionequipo = await _context.Recepcionequipos.FindAsync(id);
            if (recepcionequipo == null)
            {
                return NotFound();
            }
            ViewData["IdCliente"] = new SelectList(_context.Clientes, "IdCliente", "IdCliente", recepcionequipo.IdCliente);
            ViewData["IdServicio"] = new SelectList(_context.Servicios, "IdServicio", "IdServicio", recepcionequipo.IdServicio);

            ViewData["TipoPc"] = new SelectList(new[]
            {
                new { Value = 0, Text = "PC Torre" },
                new { Value = 1, Text = "Notebook" },
                new { Value = 2, Text = "All-In-ONE" }
            }, "Value", "Text");

            ViewData["CapacidadRam"] = new SelectList(new[]
            {
                new { Value = 0, Text = "4 GB" },
                new { Value = 1, Text = "6 GB" },
                new { Value = 2, Text = "8 GB" },
                new { Value = 3, Text = "12 GB" },
                new { Value = 4, Text = "Otra" }
            }, "Value", "Text");

            ViewData["TipoAlmacenamiento"] = new SelectList(new[]
            {
                new { Value = 0, Text = "HDD" },
                new { Value = 1, Text = "SSD Sata" },
                new { Value = 2, Text = "SSD M.2" },
                new { Value = 3, Text = "SSD NVM M.2" }
            }, "Value", "Text");

            ViewData["TipoGpu"] = new SelectList(new[]
            {
                new { Value = 0, Text = "Chip Integrado" },
                new { Value = 1, Text = "Tarjeta" }
            }, "Value", "Text");

            ViewData["Estado"] = new SelectList(new[]
            {
                new { Value = 1, Text = "Activo" },
                new { Value = 0, Text = "Inactivo" }
            }, "Value", "Text");

            return View(recepcionequipo);
        }

        // POST: Recepcionequipoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdCliente,IdServicio,Fecha,TipoPc,Accesorio,MarcaPc,ModeloPc," +
"Nserie,CapacidadRam,TipoAlmacenamiento,CapacidadAlmacenamiento,TipoGpu,Grafico,Estado")] Recepcionequipo recepcionequipo)
        {
            if (id != recepcionequipo.Id)
            {
                return NotFound();
            }

            if (recepcionequipo.IdCliente != 0) // Verificar si el modelo es válido
            {
                try
                {
                    _context.Update(recepcionequipo);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecepcionequipoExists(recepcionequipo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "No se pudieron guardar los cambios. El registro fue actualizado o eliminado por otro usuario.");
                    }
                }
                catch (Exception ex)
                {
                    // Imprimir el mensaje de error en la consola
                    Console.WriteLine($"Error al guardar en la base de datos: {ex.Message}");

                    // También puedes imprimir detalles adicionales si los necesitas
                    Console.WriteLine($"Detalles adicionales: {ex.InnerException}");

                    ModelState.AddModelError(string.Empty, $"Ocurrió un error: {ex.Message}");
                }
            }

            // Si el modelo no es válido o si ocurre un error, vuelve a cargar los datos necesarios para mostrar la vista de edición
            ViewData["IdCliente"] = new SelectList(_context.Clientes, "IdCliente", "IdCliente", recepcionequipo.IdCliente);
            ViewData["IdServicio"] = new SelectList(_context.Servicios, "IdServicio", "IdServicio", recepcionequipo.IdServicio);
            return View(recepcionequipo);
        }


        private bool RecepcionequipoExists(int id)
        {
            return _context.Recepcionequipos.Any(e => e.Id == id);
        }






        // GET: Recepcionequipoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recepcionequipo = await _context.Recepcionequipos
                .Include(r => r.IdClienteNavigation)
                .Include(r => r.IdServicioNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recepcionequipo == null)
            {
                return NotFound();
            }

            return View(recepcionequipo);
        }

        // POST: Recepcionequipoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recepcionequipo = await _context.Recepcionequipos.FindAsync(id);
            if (recepcionequipo != null)
            {
                _context.Recepcionequipos.Remove(recepcionequipo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Recepcionequipoes/Finalizar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalizar(int id)
        {
            var recepcionEquipo = await _context.Recepcionequipos.FindAsync(id);
            if (recepcionEquipo == null)
            {
                return NotFound();
            }

            recepcionEquipo.Estado = "0";

            try
            {
                _context.Update(recepcionEquipo);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecepcionequipoExists(recepcionEquipo.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

    }
}