using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using _13_DangHoangSon_Assignment01_FontEnd.Models;

namespace _13_DangHoangSon_Assignment01_FontEnd.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _client;
        private readonly string _primaryBaseUrl;
        private readonly string _fallbackBaseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService(IConfiguration configuration)
        {
            _primaryBaseUrl = configuration["ApiBaseUrl"] ?? "https://localhost:7132/";
            if (!_primaryBaseUrl.EndsWith("/")) _primaryBaseUrl += "/";

            _fallbackBaseUrl = "http://localhost:5208/";

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri(_primaryBaseUrl),
                Timeout = TimeSpan.FromSeconds(5)
            };
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        private async Task<HttpResponseMessage> SendRequestAsync(Func<string, Task<HttpResponseMessage>> requestFunc, string relativePath)
        {
            try
            {
                return await requestFunc(_primaryBaseUrl + relativePath);
            }
            catch (Exception)
            {
                try
                {
                    return await requestFunc(_fallbackBaseUrl + relativePath);
                }
                catch (Exception)
                {
                    return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                    {
                        Content = new StringContent(
                            JsonSerializer.Serialize(new { message = $"Không thể kết nối đến máy chủ BackEnd API ({_primaryBaseUrl} hoặc {_fallbackBaseUrl}). Vui lòng kiểm tra xem dự án BackEnd đã được khởi chạy chưa!" }),
                            Encoding.UTF8,
                            "application/json")
                    };
                }
            }
        }

        private Task<HttpResponseMessage> SafeGetAsync(string relativePath)
        {
            return SendRequestAsync(url => _client.GetAsync(url), relativePath);
        }

        private Task<HttpResponseMessage> SafePostAsync(string relativePath, HttpContent content)
        {
            return SendRequestAsync(async url =>
            {
                // Clone content for fallback retry
                var contentBytes = await content.ReadAsByteArrayAsync();
                var clonedContent = new ByteArrayContent(contentBytes);
                if (content.Headers.ContentType != null)
                {
                    clonedContent.Headers.ContentType = content.Headers.ContentType;
                }
                return await _client.PostAsync(url, clonedContent);
            }, relativePath);
        }

        private Task<HttpResponseMessage> SafePutAsync(string relativePath, HttpContent content)
        {
            return SendRequestAsync(async url =>
            {
                var contentBytes = await content.ReadAsByteArrayAsync();
                var clonedContent = new ByteArrayContent(contentBytes);
                if (content.Headers.ContentType != null)
                {
                    clonedContent.Headers.ContentType = content.Headers.ContentType;
                }
                return await _client.PutAsync(url, clonedContent);
            }, relativePath);
        }

        private Task<HttpResponseMessage> SafeDeleteAsync(string relativePath)
        {
            return SendRequestAsync(url => _client.DeleteAsync(url), relativePath);
        }

        private async Task<List<T>> DeserializeListAsync<T>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode) return new List<T>();
            var json = await response.Content.ReadAsStringAsync();
            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.ValueKind == JsonValueKind.Object && doc.RootElement.TryGetProperty("value", out var valueElem))
                {
                    return JsonSerializer.Deserialize<List<T>>(valueElem.GetRawText(), _jsonOptions) ?? new List<T>();
                }
                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    return JsonSerializer.Deserialize<List<T>>(json, _jsonOptions) ?? new List<T>();
                }
            }
            catch
            {
                // Fallback
            }
            return new List<T>();
        }

        private async Task<T?> DeserializeSingleAsync<T>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode) return default;
            var json = await response.Content.ReadAsStringAsync();
            try
            {
                return JsonSerializer.Deserialize<T>(json, _jsonOptions);
            }
            catch
            {
                return default;
            }
        }

        private static async Task<string> ExtractErrorMessageAsync(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.ValueKind == JsonValueKind.Object)
                {
                    if (doc.RootElement.TryGetProperty("message", out var msgProp))
                        return msgProp.GetString() ?? content;
                    if (doc.RootElement.TryGetProperty("error", out var errProp))
                    {
                        if (errProp.ValueKind == JsonValueKind.Object && errProp.TryGetProperty("message", out var errMsg))
                            return errMsg.GetString() ?? content;
                        return errProp.GetString() ?? content;
                    }
                }
            }
            catch
            {
                // Raw text
            }
            return string.IsNullOrWhiteSpace(content) ? response.ReasonPhrase ?? "Lỗi không xác định" : content;
        }

        #region Authentication
        public async Task<LoginResponse?> LoginAsync(string email, string password)
        {
            var payload = JsonSerializer.Serialize(new { email, password });
            var response = await SafePostAsync("api/Auth/login", new StringContent(payload, Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode) return null;
            return await DeserializeSingleAsync<LoginResponse>(response);
        }
        #endregion

        #region System Accounts
        public async Task<List<SystemAccountViewModel>> GetAccountsAsync(string? search = null)
        {
            var response = await SafeGetAsync("api/SystemAccounts");
            var list = await DeserializeListAsync<SystemAccountViewModel>(response);
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                list = list.Where(a => (!string.IsNullOrEmpty(a.AccountName) && a.AccountName.ToLower().Contains(term))
                                    || (!string.IsNullOrEmpty(a.AccountEmail) && a.AccountEmail.ToLower().Contains(term))).ToList();
            }
            return list;
        }

        public async Task<SystemAccountViewModel?> GetAccountByIdAsync(short id)
        {
            var response = await SafeGetAsync($"api/SystemAccounts/{id}");
            return await DeserializeSingleAsync<SystemAccountViewModel>(response);
        }

        public async Task<(bool Success, string Message)> CreateAccountAsync(SystemAccountViewModel account)
        {
            var payload = JsonSerializer.Serialize(new
            {
                accountId = account.AccountId,
                accountName = account.AccountName,
                accountEmail = account.AccountEmail,
                accountRole = account.AccountRole,
                accountPassword = account.AccountPassword
            });
            var response = await SafePostAsync("api/SystemAccounts", new StringContent(payload, Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                return (true, "Tạo tài khoản thành công!");
            }
            var err = await ExtractErrorMessageAsync(response);
            return (false, err);
        }

        public async Task<(bool Success, string Message)> UpdateAccountAsync(short id, SystemAccountViewModel account)
        {
            account.AccountId = id;
            var payload = JsonSerializer.Serialize(new
            {
                accountId = id,
                accountName = account.AccountName,
                accountEmail = account.AccountEmail,
                accountRole = account.AccountRole,
                accountPassword = account.AccountPassword
            });
            var response = await SafePutAsync($"api/SystemAccounts/{id}", new StringContent(payload, Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                return (true, "Cập nhật tài khoản thành công!");
            }
            var err = await ExtractErrorMessageAsync(response);
            return (false, err);
        }

        public async Task<(bool Success, string Message)> DeleteAccountAsync(short id)
        {
            var response = await SafeDeleteAsync($"api/SystemAccounts/{id}");
            if (response.IsSuccessStatusCode)
            {
                return (true, "Xóa tài khoản thành công!");
            }
            var err = await ExtractErrorMessageAsync(response);
            return (false, err);
        }
        #endregion

        #region Categories
        public async Task<List<CategoryViewModel>> GetCategoriesAsync(string? search = null)
        {
            var response = await SafeGetAsync("api/Categories?$expand=ParentCategory");
            var list = await DeserializeListAsync<CategoryViewModel>(response);
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                list = list.Where(c => (!string.IsNullOrEmpty(c.CategoryName) && c.CategoryName.ToLower().Contains(term))
                                    || (!string.IsNullOrEmpty(c.CategoryDesciption) && c.CategoryDesciption.ToLower().Contains(term))).ToList();
            }
            return list;
        }

        public async Task<CategoryViewModel?> GetCategoryByIdAsync(short id)
        {
            var response = await SafeGetAsync($"api/Categories/{id}");
            return await DeserializeSingleAsync<CategoryViewModel>(response);
        }

        public async Task<(bool Success, string Message)> CreateCategoryAsync(CategoryViewModel category)
        {
            var payload = JsonSerializer.Serialize(new
            {
                categoryName = category.CategoryName,
                categoryDesciption = category.CategoryDesciption,
                parentCategoryId = category.ParentCategoryId,
                isActive = category.IsActive ?? true
            });
            var response = await SafePostAsync("api/Categories", new StringContent(payload, Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                return (true, "Tạo danh mục thành công!");
            }
            var err = await ExtractErrorMessageAsync(response);
            return (false, err);
        }

        public async Task<(bool Success, string Message)> UpdateCategoryAsync(short id, CategoryViewModel category)
        {
            category.CategoryId = id;
            var payload = JsonSerializer.Serialize(new
            {
                categoryId = id,
                categoryName = category.CategoryName,
                categoryDesciption = category.CategoryDesciption,
                parentCategoryId = category.ParentCategoryId,
                isActive = category.IsActive ?? true
            });
            var response = await SafePutAsync($"api/Categories/{id}", new StringContent(payload, Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                return (true, "Cập nhật danh mục thành công!");
            }
            var err = await ExtractErrorMessageAsync(response);
            return (false, err);
        }

        public async Task<(bool Success, string Message)> DeleteCategoryAsync(short id)
        {
            var response = await SafeDeleteAsync($"api/Categories/{id}");
            if (response.IsSuccessStatusCode)
            {
                return (true, "Xóa danh mục thành công!");
            }
            var err = await ExtractErrorMessageAsync(response);
            return (false, err);
        }
        #endregion

        #region News Articles
        public async Task<List<NewsArticleViewModel>> GetNewsArticlesAsync(string? search = null, short? categoryId = null, bool activeOnly = false)
        {
            var response = await SafeGetAsync("api/NewsArticles?$expand=Category,CreatedBy,Tags&$orderby=CreatedDate desc");
            var list = await DeserializeListAsync<NewsArticleViewModel>(response);

            if (activeOnly)
            {
                list = list.Where(a => a.NewsStatus == true).ToList();
            }
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                list = list.Where(a => a.CategoryId == categoryId.Value).ToList();
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                list = list.Where(a => (!string.IsNullOrEmpty(a.NewsTitle) && a.NewsTitle.ToLower().Contains(term))
                                    || (!string.IsNullOrEmpty(a.Headline) && a.Headline.ToLower().Contains(term))
                                    || (!string.IsNullOrEmpty(a.NewsContent) && a.NewsContent.ToLower().Contains(term))).ToList();
            }
            return list;
        }

        public async Task<NewsArticleViewModel?> GetNewsArticleByIdAsync(string id)
        {
            var response = await SafeGetAsync($"api/NewsArticles/{id}?$expand=Category,CreatedBy,Tags");
            return await DeserializeSingleAsync<NewsArticleViewModel>(response);
        }

        public async Task<(bool Success, string Message)> CreateNewsArticleAsync(NewsArticleEditViewModel article)
        {
            var payload = JsonSerializer.Serialize(new
            {
                newsArticleId = article.NewsArticleId,
                newsTitle = article.NewsTitle,
                headline = article.Headline,
                newsContent = article.NewsContent,
                newsSource = article.NewsSource,
                categoryId = article.CategoryId,
                newsStatus = article.NewsStatus ?? true,
                createdById = article.CreatedById,
                updatedById = article.UpdatedById,
                tagIds = article.TagIds ?? new List<int>()
            });
            var response = await SafePostAsync("api/NewsArticles", new StringContent(payload, Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                return (true, "Tạo bài viết thành công!");
            }
            var err = await ExtractErrorMessageAsync(response);
            return (false, err);
        }

        public async Task<(bool Success, string Message)> UpdateNewsArticleAsync(string id, NewsArticleEditViewModel article)
        {
            article.NewsArticleId = id;
            var payload = JsonSerializer.Serialize(new
            {
                newsArticleId = article.NewsArticleId,
                newsTitle = article.NewsTitle,
                headline = article.Headline,
                newsContent = article.NewsContent,
                newsSource = article.NewsSource,
                categoryId = article.CategoryId,
                newsStatus = article.NewsStatus ?? true,
                createdById = article.CreatedById,
                updatedById = article.UpdatedById,
                tagIds = article.TagIds ?? new List<int>()
            });
            var response = await SafePutAsync($"api/NewsArticles/{id}", new StringContent(payload, Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                return (true, "Cập nhật bài viết thành công!");
            }
            var err = await ExtractErrorMessageAsync(response);
            return (false, err);
        }

        public async Task<(bool Success, string Message)> DeleteNewsArticleAsync(string id)
        {
            var response = await SafeDeleteAsync($"api/NewsArticles/{id}");
            if (response.IsSuccessStatusCode)
            {
                return (true, "Xóa bài viết thành công!");
            }
            var err = await ExtractErrorMessageAsync(response);
            return (false, err);
        }
        #endregion

        #region Tags
        public async Task<List<TagViewModel>> GetTagsAsync()
        {
            var response = await SafeGetAsync("api/Tags");
            return await DeserializeListAsync<TagViewModel>(response);
        }
        #endregion

        #region Report
        public async Task<List<NewsArticleViewModel>> GetReportAsync(DateTime startDate, DateTime endDate)
        {
            var s = startDate.ToString("yyyy-MM-dd");
            var e = endDate.ToString("yyyy-MM-dd");
            var response = await SafeGetAsync($"api/NewsArticles/report?startDate={s}&endDate={e}");
            return await DeserializeListAsync<NewsArticleViewModel>(response);
        }
        #endregion
    }
}
