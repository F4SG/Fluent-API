using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore; // Necesario para usar AnyAsync
using HeroesWeb.Data;
using HeroesWeb.Models;

namespace HeroesWeb.Pages_SuperPoderes
{
    public class CreateModel : PageModel
    {
        private readonly HeroesWeb.Data.HeroesContext _context;

        public CreateModel(HeroesWeb.Data.HeroesContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            // CAMBIO 10.1: Mostrar "Nombre" en lugar de "Id"
            ViewData["HeroeId"] = new SelectList(_context.Heroes, "Id", "Nombre");
            return Page();
        }

        [BindProperty]
        public SuperPoderes SuperPoderes { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            // CAMBIO 10.3: Validar que el héroe exista en la base de datos
            if (!await _context.Heroes.AnyAsync(h => h.Id == SuperPoderes.HeroeId))
            {
                ModelState.AddModelError("SuperPoderes.HeroeId", "Seleccione un héroe válido.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["HeroeId"] = new SelectList(
                    _context.Heroes, "Id", "Nombre", SuperPoderes.HeroeId);
                return Page();
            }

            _context.SuperPoderes.Add(SuperPoderes);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}