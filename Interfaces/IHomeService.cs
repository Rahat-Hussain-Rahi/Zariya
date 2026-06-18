using Zariya.ViewModels;

namespace Zariya.Interfaces;

public interface IHomeService
{
    Task<HomePageViewModel> GetHomePageAsync();
}
