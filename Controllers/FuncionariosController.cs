﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pimfo.Models;
using pimfo.data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace pimfo.Controllers
{
    [Authorize]
    public class FuncionariosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationDbContext _contextLogin;    
        //ATUAL AAAAAAAAAAAA
        public FuncionariosController(ApplicationDbContext context, ApplicationDbContext contextLogin)
        {
            _context = context;
            _contextLogin = contextLogin;
        }

        // GET: Funcionarios
        public async Task<IActionResult> Index()
        {

            return _context.Funcionarios != null ? 
                          View(await _context.Funcionarios.OrderByDescending(item=>item).ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Funcionarios'  is null.");
        }

        // GET: Funcionarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Funcionarios == null)
            {
                return NotFound();
            }

            var funcionarios = await _context.Funcionarios
                .FirstOrDefaultAsync(m => m.id == id);
            if (funcionarios == null)
            {
                return NotFound();
            }

            return View(funcionarios);
        }

        // GET: Funcionarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Funcionarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,nome,rg,cpf,data_nasc,endereco,cargo,salario_bruto, rh")] Funcionarios funcionarios)
        {//aaaaaaaaaaaaaa
            if (ModelState.IsValid)
            {

   
                var login = new Login
                {
                    id_func = funcionarios.id,
                    usuario = funcionarios.nome,
                    senha = funcionarios.cpf,
                    rh = funcionarios.rh
                };
                _context.Login.Add(login);

                _context.Add(funcionarios);
                await _context.SaveChangesAsync();

                await _contextLogin.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(funcionarios);
        }

        // GET: Funcionarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Funcionarios == null)
            {
                return NotFound();
            }

            var funcionarios = await _context.Funcionarios.FindAsync(id);
            if (funcionarios == null)
            {
                return NotFound();
            }
            return View(funcionarios);
        }

        // POST: Funcionarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,nome,rg,cpf,data_nasc,endereco,cargo,salario_bruto")] Funcionarios funcionarios)
        {
            if (id != funcionarios.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(funcionarios);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FuncionariosExists(funcionarios.id))
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
            return View(funcionarios);
        }

        // GET: Funcionarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Funcionarios == null)
            {
                return NotFound();
            }

            var funcionarios = await _context.Funcionarios
                .FirstOrDefaultAsync(m => m.id == id);
            if (funcionarios == null)
            {
                return NotFound();
            }

            return View(funcionarios);
        }

        // POST: Funcionarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Funcionarios == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Funcionarios'  is null.");
            }
            var funcionarios = await _context.Funcionarios.FindAsync(id);
            if (funcionarios != null)
            {
                _context.Funcionarios.Remove(funcionarios);
                //Deletar usuario
                var login = await _contextLogin.Login.FindAsync(id);
                _contextLogin.Login.Remove(login);

            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Adiantamento()
        {
            return View();
        }

        public async Task<IActionResult> Gerar_Adiantamento(string cpf, float valor_adiantamento)
        {
            var funcionario = _context.Funcionarios.FirstOrDefault(e=> e.cpf == cpf);
            if (funcionario != null)
            {
                funcionario.valor_adiantamento = valor_adiantamento;
                _context.SaveChanges();
                return RedirectToAction("Index", "Funcionarios");
            }

            return RedirectToAction("Index", "Funcionarios");
        }

        private bool FuncionariosExists(int id)
        {
          return (_context.Funcionarios?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
