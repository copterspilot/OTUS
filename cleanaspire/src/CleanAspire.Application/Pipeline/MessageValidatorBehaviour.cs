// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentValidation.Results;

namespace CleanAspire.Application.Pipeline;
public sealed class MessageValidatorBehaviour<TMessage, TResponse> : MessagePreProcessor<TMessage, TResponse>
    where TMessage : IMessage, IRequiresValidation
{
    private readonly IReadOnlyCollection<IValidator<TMessage>> _validators;

    public MessageValidatorBehaviour(IEnumerable<IValidator<TMessage>> validators)
    {
        _validators = validators.ToList() ?? throw new ArgumentNullException(nameof(validators));
    }

    protected override async ValueTask Handle(TMessage message,
        CancellationToken cancellationToken
       )
    {
        var context = new ValidationContext<TMessage>(message);
        var validationResult = await _validators.ValidateAsync(context, cancellationToken);
        if (validationResult.Any())
        {
            throw new ValidationException(validationResult);
        }

    }
}


public static class ValidationExtensions
{
    public static async Task<List<ValidationFailure>> ValidateAsync<TRequest>(
        this IEnumerable<IValidator<TRequest>> validators, ValidationContext<TRequest> validationContext,
        CancellationToken cancellationToken = default)
    {
        if (!validators.Any()) return new List<ValidationFailure>();

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(validationContext, cancellationToken)));

        return validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();
    }

    public static Dictionary<string, string[]> ToDictionary(this List<ValidationFailure>? failures)
    {
        return failures != null && failures.Any()
            ? failures.GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(g => g.Key, g => g.ToArray())
            : new Dictionary<string, string[]>();
    }
}
public interface IRequiresValidation
{
}
