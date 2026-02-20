using PP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Domain.Interfaces
{
	public interface IRefreshTokenRepository
	{
		Task AddAsync(RefreshToken refreshToken);
		Task<RefreshToken?> GetByTokenAsync(string token);
		Task UpdateAsync(RefreshToken refreshToken);
		Task DeleteAsync(RefreshToken refreshToken);
	}
}
