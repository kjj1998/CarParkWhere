using CZ3002_Backend.Enums;
using CZ3002_Backend.Models;
using Google.Cloud.Firestore;

namespace CZ3002_Backend.Repo;

public class UserRepository:IUserRepository
{
    private readonly IHdbCarparkRepository _hdbCarparkRepository;
    private readonly IMallCarparkRepository _mallCarparkRepository;
    private readonly IUraCarparkRepository _uraCarparkRepository;
    private readonly BaseRepository<UserModel> _repository;

    public UserRepository(IConfiguration configuration, IHdbCarparkRepository hdbCarparkRepository, IMallCarparkRepository mallCarparkRepository, IUraCarparkRepository uraCarparkRepository)
    {
        _hdbCarparkRepository = hdbCarparkRepository;
        _mallCarparkRepository = mallCarparkRepository;
        _uraCarparkRepository = uraCarparkRepository;
        
        _repository = new BaseRepository<UserModel>(Collection.Users,configuration);
        
    }
    public async Task<List<UserModel>> GetAllAsync() => await _repository.GetAllAsync<UserModel>(); 

    public async Task<UserModel> GetAsync(UserModel entity) => (UserModel) await _repository.GetAsync(entity);

    public async Task<UserModel> AddAsync(UserModel entity) => await _repository.AddAsync(entity);

    public async Task<List<UserModel>> AddMultipleAsync(List<UserModel> listOfEntities) =>
        await _repository.AddMultipleAsync(listOfEntities);

    public async Task<UserModel> UpdateAsync(UserModel entity) => await _repository.UpdateAsync(entity);

    public async Task DeleteAsync(UserModel entity) => await _repository.DeleteAsync(entity);

    public async Task<List<UserModel>> QueryRecordsAsync(Query query) => await _repository.QueryRecordsAsync<UserModel>(query);
    
    // This is specific to User.
    public async Task<UserModel> GetUserFavouriteCarParks(string user)
    {
        var query = (_repository)._firestoreDb.Collection(Collection.Users.ToString()).WhereEqualTo(nameof(UserModel.Name), user);
        var userModels =  await this.QueryRecordsAsync(query);
        if (userModels.Count != 0)
        {
            var hdbQuery = (_hdbCarparkRepository.GetBaseRepository())._firestoreDb.Collection(Collection.HdbCarparks.ToString()).WhereIn(nameof(HdbCarParkModel.CarparkCode), userModels[0].FavouriteHDBCarParkCodes);
            var mallQuery = (_mallCarparkRepository.GetBaseRepository())._firestoreDb.Collection(Collection.MallCarparks.ToString()).WhereIn(nameof(MallCarparkModel.CarparkCode), userModels[0].FavouriteMallCarParkCodes);
            var uraQuery = (_uraCarparkRepository.GetBaseRepository())._firestoreDb.Collection(Collection.UraCarparks.ToString()).WhereIn(nameof(UraCarparkModel.CarparkCode), userModels[0].FavouriteURACarParkCodes);

            if (userModels[0].FavouriteHDBCarParkCodes != null && userModels[0].FavouriteHDBCarParkCodes.Count > 0)
            {
                userModels[0].FavouriteHDBCarParks = await _hdbCarparkRepository.QueryRecordsAsync(hdbQuery);
            }

            if (userModels[0].FavouriteMallCarParkCodes != null && userModels[0].FavouriteMallCarParkCodes.Count > 0)
            {
                userModels[0].FavouriteMallCarParks = await _mallCarparkRepository.QueryRecordsAsync(mallQuery);
            }

            if (userModels[0].FavouriteURACarParkCodes != null && userModels[0].FavouriteURACarParkCodes.Count > 0)
            {
                userModels[0].FavouriteURACarParks = await _uraCarparkRepository.QueryRecordsAsync(uraQuery);
            }

            return userModels[0];
        }
        
        return new UserModel();
    }

    public async Task UpsertUserFavouriteCarPark(string user, string carParkCode)
    {
        var query = (_repository)._firestoreDb.Collection(Collection.Users.ToString()).WhereEqualTo(nameof(UserModel.Name), user);
        var userModels =  await QueryRecordsAsync(query);
        UserModel userModel;
        if (userModels == null || userModels.Count == 0)
        {
            userModel = new UserModel()
            {
                Name = user,
                FavouriteHDBCarParkCodes = new List<string>(),
                FavouriteHDBCarParks = new List<HdbCarParkModel>(),
                FavouriteMallCarParkCodes = new List<string>(),
                FavouriteMallCarParks = new List<MallCarparkModel>(),
                FavouriteURACarParkCodes = new List<string>(),
                FavouriteURACarParks = new List<UraCarparkModel>()
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
        var query = (_repository)._firestoreDb.Collection(Collection.Users.ToString()).WhereEqualTo(nameof(UserModel.Name), user);
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

}