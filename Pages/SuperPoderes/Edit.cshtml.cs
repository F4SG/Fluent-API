using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HeroesWeb.Data;
using HeroesWeb.Models;

namespace HeroesWeb.Pages_SuperPoderes
{
    public class EditModel : PageModel
    {
        private readonly HeroesWeb.Data.HeroesContext _context;

        public EditModel(HeroesWeb.Data.HeroesContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SuperPoderes SuperPoderes { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var superpoderes = await _context.SuperPoderes.FirstOrDefaultAsync(m => m.Id == id);
            if (superpoderes == null)
            {
                return NotFound();
            }
            SuperPoderes = superpoderes;

            // CAMBIO 10.1: Mostrar "Nombre" y preservar la selección actual del héroe al cargar el formulario
            ViewData["HeroeId"] = new SelectList(_context.Heroes, "Id", "Nombre", SuperPoderes.HeroeId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // CAMBIO 10.3: Validar que el héroe exista
            if (!await _context.Heroes.AnyAsync(h => h.Id == SuperPoderes.HeroeId))
            {
                ModelState.AddModelError("SuperPoderes.HeroeId", "Seleccione un héroe válido.");
            }

            if (!ModelState.IsValid)
            {
                // CAMBIO 10.3: Preservar la selección si el formulario tiene errores al guardar
                ViewData["HeroeId"] = new SelectList(
                    _context.Heroes, "Id", "Nombre", SuperPoderes.HeroeId);
                return Page();
            }

            _context.Attach(SuperPoderes).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SuperPoderesExists(SuperPoderes.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool SuperPoderesExists(int id)
        {
            return _context.SuperPoderes.Any(e => e.Id == id);
        }
    }
}