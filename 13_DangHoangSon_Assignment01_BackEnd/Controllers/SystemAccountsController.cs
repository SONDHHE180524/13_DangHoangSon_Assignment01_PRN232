using _13_DangHoangSon_Assignment01_BackEnd.DTOs;
using _13_DangHoangSon_Assignment01_BackEnd.Models;
using _13_DangHoangSon_Assignment01_BackEnd.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace _13_DangHoangSon_Assignment01_BackEnd.Controllers
{
    [Route("api/SystemAccounts")]
    [ApiController]
    public class SystemAccountsController : ControllerBase
    {
        private readonly ISystemAccountRepository _accountRepo;

        public SystemAccountsController(ISystemAccountRepository accountRepo)
        {
            _accountRepo = accountRepo;
        }

        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var accounts = await _accountRepo.GetAllAccountsAsync();
            return Ok(accounts.AsQueryable());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(short id)
        {
            var account = await _accountRepo.GetAccountByIdAsync(id);
            if (account == null) return NotFound("Không tìm thấy tài khoản.");
            return Ok(account);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AccountRequestDto request)
        {
            if (request == null || !ModelState.IsValid) return BadRequest("Dữ liệu tài khoản không hợp lệ.");

            var existing = await _accountRepo.GetAccountByEmailAsync(request.AccountEmail ?? "");
            if (existing != null)
            {
                return BadRequest("Email này đã được sử dụng.");
            }

            var account = new SystemAccount
            {
                AccountId = request.AccountId,
                AccountName = request.AccountName,
                AccountEmail = request.AccountEmail,
                AccountRole = request.AccountRole,
                AccountPassword = request.AccountPassword
            };

            await _accountRepo.AddAccountAsync(account);
            return Ok(account);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(short id, [FromBody] AccountRequestDto request)
        {
            if (request == null) return BadRequest("Dữ liệu tài khoản không hợp lệ.");
            if (id != request.AccountId && request.AccountId != 0) request.AccountId = id;

            var existing = await _accountRepo.GetAccountByIdAsync(id);
            if (existing == null) return NotFound("Không tìm thấy tài khoản.");

            var account = new SystemAccount
            {
                AccountId = id,
                AccountName = request.AccountName ?? existing.AccountName,
                AccountEmail = request.AccountEmail ?? existing.AccountEmail,
                AccountRole = request.AccountRole ?? existing.AccountRole,
                AccountPassword = string.IsNullOrEmpty(request.AccountPassword) ? existing.AccountPassword : request.AccountPassword
            };

            await _accountRepo.UpdateAccountAsync(account);
            return Ok(account);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(short id)
        {
            var deleted = await _accountRepo.DeleteAccountAsync(id);
            if (!deleted)
            {
                return BadRequest("Không thể xóa tài khoản vì tài khoản này đã từng tạo bài viết tin tức!");
            }
            return NoContent();
        }
    }
}