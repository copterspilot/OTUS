// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
global using CleanAspire.Domain;
global using Microsoft.EntityFrameworkCore;
global using CleanAspire.Application.Common.Interfaces;
global using CleanAspire.Application.Common.Interfaces.FusionCache;
global using CleanAspire.Application.Common.Models;
global using CleanAspire.Application.Common;
global using CleanAspire.Domain.Entities;
global using FluentValidation;
global using Mediator;
global using Microsoft.Extensions.Logging;
global using ZiggyCreatures.Caching.Fusion;
