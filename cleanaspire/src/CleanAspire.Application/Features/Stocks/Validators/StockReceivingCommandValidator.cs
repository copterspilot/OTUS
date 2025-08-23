// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanAspire.Application.Features.Stocks.Commands;

namespace CleanAspire.Application.Features.Stocks.Validators;
public class StockReceivingCommandValidator : AbstractValidator<StockReceivingCommand>
{
    public StockReceivingCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("ProductId is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0.");

        RuleFor(x => x.Location)
            .NotEmpty()
            .WithMessage("Location is required.");
    }
}
