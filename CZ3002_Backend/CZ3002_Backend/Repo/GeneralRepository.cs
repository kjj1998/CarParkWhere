using CZ3002_Backend.Enums;
using CZ3002_Backend.Models;
using Google.Cloud.Firestore;

namespace CZ3002_Backend.Repo;

public class GeneralRepository:IGeneralRepository
{
    private readonly IHdbCarparkRepository _hdbCarparkRepository;
    private readonly IMallCarparkRepository _mallCarparkRepository;
    private readonly IUraCarparkRepository _uraCarparkRepository;
    private readonly BaseRepository<UserModel> _userRepository;

    public GeneralRepository(IConfiguration configuration, IHdbCarparkRepository hdbCarparkRepository, IMallCarparkRepository mallCarparkRepository, IUraCarparkRepository uraCarparkRepository)
    {
        _hdbCarparkRepository = hdbCarparkRepository;
        _mallCarparkRepository = mallCarparkRepository;
        _uraCarparkRepository = uraCarparkRepository;
        
        _userRepository = new BaseRepository<UserModel>(Collection.Users,configuration);
        
    }
    public async Task<List<UserModel>> GetAllAsync() => await _userRepository.GetAllAsync<UserModel>(); 

    public async Task<UserModel> GetAsync(UserModel entity) => (UserModel) await _userRepository.GetAsync(entity);

    public async Task<UserModel> AddAsync(UserModel entity) => await _userRepository.AddAsync(entity);

    public async Task<List<UserModel>> AddMultipleAsync(List<UserModel> listOfEntities) =>
        await _userRepository.AddMultipleAsync(listOfEntities);

    public async Task<UserModel> UpdateAsync(UserModel entity) => await _userRepository.UpdateAsync(entity);

    public async Task DeleteAsync(UserModel entity) => await _userRepository.DeleteAsync(entity);

    public async Task<List<UserModel>> QueryRecordsAsync(Query query) => await _userRepository.QueryRecordsAsync<UserModel>(query);
    
    // This is specific to User.
    public async Task<UserModel> GetUserFavouriteCarParks(string user)
    {
        var query = (_userRepository)._firestoreDb.Collection(Collection.Users.ToString()).WhereEqualTo(nameof(UserModel.Name), user);
        var userModels =  await this.QueryRecordsAsync(query);
        if (userModels.Count != 0)
        {
            var hdbQuery = (_hdbCarparkRepository.GetBaseRepository())._firestoreDb.Collection(Collection.HdbCarparks.ToString()).WhereIn(nameof(HdbCarParkModel.CarparkCode), userModels[0].FavouriteHDBCarParkCodes);
            var mallQuery = (_mallCarparkRepository.GetBaseRepository())._firestoreDb.Collection(Collection.MallCarparks.ToString()).WhereIn(nameof(MallCarparkModel.CarparkCode), userModels[0].FavouriteMallCarParkCodes);
            var uraQuery = (_uraCarparkRepository.GetBaseRepository())._firestoreDb.Collection(Collection.UraCarparks.ToString()).WhereIn(nameof(UraCarparkModel.CarparkCode), userModels[0].FavouriteURACarParkCodes);

            if (userModels[0].FavouriteHDBCarParkCodes != null && userModels[0].FavouriteHDBCarParkCodes.Count > 0)
            {
                userModels[0].Hdb = await _hdbCarparkRepository.QueryRecordsAsync(hdbQuery);
            }

            if (userModels[0].FavouriteMallCarParkCodes != null && userModels[0].FavouriteMallCarParkCodes.Count > 0)
            {
                userModels[0].Mall = await _mallCarparkRepository.QueryRecordsAsync(mallQuery);
            }

            if (userModels[0].FavouriteURACarParkCodes != null && userModels[0].FavouriteURACarParkCodes.Count > 0)
            {
                userModels[0].Ura = await _uraCarparkRepository.QueryRecordsAsync(uraQuery);
            }

            return userModels[0];
        }
        
        return new UserModel();
    }

    public async Task UpsertUserFavouriteCarPark(string user, string carParkCode)
    {
        var query = (_userRepository)._firestoreDb.Collection(Collection.Users.ToString()).WhereEqualTo(nameof(UserModel.Name), user);
        var userModels =  await QueryRecordsAsync(query);
        UserModel userModel;
        if (userModels == null || userModels.Count == 0)
        {
            userModel = new UserModel()
            {
                Name = user,
                FavouriteHDBCarParkCodes = new List<string>(),
                Hdb = new List<HdbCarParkModel>(),
                FavouriteMallCarParkCodes = new List<string>(),
                Mall = new List<MallCarparkModel>(),
                FavouriteURACarParkCodes = new List<string>(),
                Ura = new List<UraCarparkModel>()
            };
        }
        else
        {
            userModel = userModels[0];
        }

        if (carParkCode.Length == 5)
        {
            if (!userModel.FavouriteURACarParkCodes.Contains(carParkCode))
            {
                userModel.FavouriteURACarParkCodes.Add(carParkCode);
            }
        }
        else if (int.TryParse(carParkCode, out _))
        {
            if (!userModel.FavouriteMallCarParkCodes.Contains(carParkCode))
            {
                userModel.FavouriteMallCarParkCodes.Add(carParkCode);
            }
        }
        else
        {
            if (!userModel.FavouriteHDBCarParkCodes.Contains(carParkCode))
            {
                userModel.FavouriteHDBCarParkCodes.Add(carParkCode);
            }
        }

        if (userModels == null || userModels.Count == 0)
        {
            await AddAsync(userModel);
        }
        else
        {
            await UpdateAsync(userModel);
        }

    }

    public async Task DeleteUserFavouriteCarPark(string user, string carParkCode)
    {
        var query = (_userRepository)._firestoreDb.Collection(Collection.Users.ToString()).WhereEqualTo(nameof(UserModel.Name), user);
        var userModels =  await QueryRecordsAsync(query);
        
        if (userModels == null || userModels.Count == 0)
        {
            return;
        }

        var userModel = userModels[0];
        
        if (carParkCode.Length == 5)
        {
            userModel.FavouriteURACarParkCodes.Remove(carParkCode);
        }
        else if (int.TryParse(carParkCode, out _))
        {
            userModel.FavouriteMallCarParkCodes.Remove(carParkCode);
        }
        else
        {
            userModel.FavouriteHDBCarParkCodes.Remove(carParkCode);
        }
        
        await UpdateAsync(userModel);
    }
    
    // This is specific to Frontend Single Car park query.
    
    public async Task<FrontendSingleCarparkModel> GetSingleCarPark(string carParkCode)
    {
        var codeList = new List<string>()
        {
            carParkCode
        };
        var uraQuery = (_uraCarparkRepository.GetBaseRepository())._firestoreDb.Collection(Collection.UraCarparks.ToString()).WhereIn(nameof(UraCarparkModel.CarparkCode), codeList);
        var mallQuery = (_mallCarparkRepository.GetBaseRepository())._firestoreDb.Collection(Collection.MallCarparks.ToString()).WhereIn(nameof(MallCarparkModel.CarparkCode), codeList);
        var hdbQuery = (_hdbCarparkRepository.GetBaseRepository())._firestoreDb.Collection(Collection.HdbCarparks.ToString()).WhereIn(nameof(HdbCarParkModel.CarparkCode), codeList);

        var frontendSingleCarparkModel = new FrontendSingleCarparkModel();
        if (carParkCode.Length == 5)
        {
            var listOfCarParks = await _uraCarparkRepository.QueryRecordsAsync(uraQuery);
            if (listOfCarParks != null && listOfCarParks.Count > 0)
            {
                frontendSingleCarparkModel.Ura = listOfCarParks[0];
            }
        }
        else if (int.TryParse(carParkCode, out _))
        {
            var listOfCarParks = await _mallCarparkRepository.QueryRecordsAsync(mallQuery);
            if (listOfCarParks != null && listOfCarParks.Count > 0)
            {
                frontendSingleCarparkModel.Mall = listOfCarParks[0];
            }
        }
        else
        {
            var listOfCarParks = await _hdbCarparkRepository.QueryRecordsAsync(hdbQuery);
            if (listOfCarParks != null && listOfCarParks.Count > 0)
            {
                frontendSingleCarparkModel.Hdb = listOfCarParks[0];
            }
        }
        
        
        
        return frontendSingleCarparkModel;
    }

}