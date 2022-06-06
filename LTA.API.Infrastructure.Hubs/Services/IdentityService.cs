﻿using LTA.API.Domain.Interfaces;
using LTA.API.Infrastructure.Hubs.Interfaces;
using LTA.API.Infrastructure.Loggers.Interfaces;

namespace LTA.API.Infrastructure.Hubs.Services;

public class IdentityService : IIdentityService
{
    private readonly IUserRepository _userRepository;
    private readonly IProfileRepository _profileRepository;
    private readonly ILoggerService _logger;

    public IdentityService(IProfileRepository profileRepository, IUserRepository userRepository, ILoggerService logger)
    {
        _profileRepository = profileRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public Task? RegisterAsync(string phoneOrEmail, string password, string confirm, string keyWord)
    {
        if (_profileRepository.GetAll().Any(p => p.Phone == phoneOrEmail || p.Email == phoneOrEmail))
        {
            throw new InvalidOperationException("You have an account already. Do you forgot your password?");
        }

        const string phone = "phone";
        const string email = "email";

        try
        {
            return keyWord.ToLower() switch
            {
                phone => _profileRepository.CreateAsync(phoneOrEmail, null, password, confirm),
                email => _profileRepository.CreateAsync(null, phoneOrEmail, password, confirm),
                _ => throw new ArgumentException($"{nameof(keyWord)} has invalid value")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Source}: {ex.Message}");
            return null;
        }
    }

    public async Task<string?> LoginAsync(string phoneOrEmail, string password)
    {
        try
        {
            var profile = await _profileRepository.GetAsync(phoneOrEmail) ??
                          throw new NullReferenceException(
                              $"Cannot find profile with phone or email like {phoneOrEmail}\nWanna sign in?");

            if (!profile.Password.Equals(password))
            {
                throw new ArgumentException($"Неверный пароль. Забыли пароль?");
            }

            var user = await _userRepository.GetAsync(profile.Id);

            if (user != null)
            {
                var updatingUser = await _userRepository.UpdateAndReturnAsync(profile.Id);
                return updatingUser.Code;
            }

            await _userRepository.CreateAsync(profile);

            return user?.Code;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Source}: {ex.Message}");
            return null;
        }
    }

    //public async Task<string> LoginAsync(string phoneOrEmail, string password)
    //{
    //    var profile = await _profileRepository.GetAsync(phoneOrEmail) ??
    //                  throw new NullReferenceException(
    //                      $"Cannot find profile with phone or email like {phoneOrEmail}\nWanna sign in?");

    //    if (profile.Password != password)
    //    {
    //        throw new ArgumentException($"Invalid password. Do you forgot your password?");
    //    }

    //    try
    //    {
    //        var user = await _userRepository.GetAsync(profile.Id);
    //        if (user != null)
    //        {
    //            return (await _userRepository.UpdateAndReturnAsync(profile.Id)).Code;
    //        }

    //        await _userRepository.CreateAsync(profile);
    //        try
    //        {
    //            await _profileRepository.UpdateAsync(profile.Id);
    //        }
    //        catch
    //        {
    //            await _userRepository.TryDeleteAsync(profile.Id);
    //        }

    //        return (await _userRepository.UpdateAndReturnAsync(profile.Id)).Code;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new NullReferenceException($"{ex}: Something wrong with login... Please, try again.");
    //    }
    //}
}