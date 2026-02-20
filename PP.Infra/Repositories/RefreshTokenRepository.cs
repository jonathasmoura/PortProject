using Microsoft.EntityFrameworkCore;
using PP.Domain.Entities;
using PP.Domain.Interfaces;
using PP.Infra.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Infra.Repositories
{
	public class RefreshTokenRepository : IRefreshTokenRepository
	{
		private readonly DbPortContexts _context;

		public RefreshTokenRepository(DbPortContexts context)
		{
			_context = context;
		}

		public async Task AddAsync(RefreshToken refreshToken)
		{
			_context.RefreshTokens.Add(refreshToken);
			await _context.SaveChangesAsync();
		}

		public async Task<RefreshToken?> GetByTokenAsync(string token)
		{
			return await _context.RefreshTokens
				.AsNoTracking()
				.FirstOrDefaultAsync(r => r.Token == token);
		}

		public async Task UpdateAsync(RefreshToken refreshToken)
		{
			_context.RefreshTokens.Update(refreshToken);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(RefreshToken refreshToken)
		{
			_context.RefreshTokens.Remove(refreshToken);
			await _context.SaveChangesAsync();
		}
	}
}
