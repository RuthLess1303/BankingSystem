﻿using BankingSystem.Atm.Core.Repositories;
using BankingSystem.Atm.Core.Validations;
using InternetBank.Db.Db.Entities;

namespace BankingSystem.Atm.Core.Services;

public interface ICardAuthService
{
    Task<AccountEntity> GetAuthorizedAccountAsync(string cardNumber, string pin);
}

public class CardAuthService : ICardAuthService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICardRepository _cardRepository;
    private readonly IWithdrawalRequestValidation _requestValidation;

    public CardAuthService(
        IAccountRepository accountRepository,
        ICardRepository cardRepository,
        IWithdrawalRequestValidation requestValidation)
    {
        _accountRepository = accountRepository;
        _cardRepository = cardRepository;
        _requestValidation = requestValidation;
    }

    public async Task<AccountEntity> GetAuthorizedAccountAsync(string cardNumber, string pin)
    {
        _requestValidation.ValidateCreditCardNumber(cardNumber);
        _requestValidation.ValidatePinCode(pin);

        var card = await _cardRepository.FindCardEntityByCardNumberAsync(cardNumber);

        if (card.ExpirationDate <= DateTime.UtcNow) throw new UnauthorizedAccessException("Card has expired");

        var account = await _accountRepository.GetAccountByCardDetails(cardNumber, pin);

        return account;
    }
}