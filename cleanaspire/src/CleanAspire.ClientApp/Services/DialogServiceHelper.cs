// This class provides helper methods to streamline the usage of the dialog service in a Blazor application.
// It encapsulates dialog interactions, simplifying the process of showing confirmation and custom dialogs.

// Purpose:
// 1. **Reusable Dialog Logic**:
//    - Provides a unified interface for showing confirmation and custom dialogs with optional callback actions.
//    - Reduces repetitive code by centralizing dialog configurations and result handling.

// 2. **User Interaction**:
//    - Enhances user experience by offering configurable dialogs for confirmations or custom content components.
//    - Supports optional callback actions for both confirmation and cancellation scenarios.


using CleanAspire.ClientApp.Components;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CleanAspire.ClientApp.Services;

// This class represents a helper class for the dialog service.
public class DialogServiceHelper
{
    private readonly IDialogService _dialogService;

    public DialogServiceHelper(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    /// <summary>
    /// Shows a confirmation dialog with the specified title and content text.
    /// </summary>
    /// <param name="title">The title of the confirmation dialog.</param>
    /// <param name="contentText">The content text of the confirmation dialog.</param>
    /// <param name="onConfirm">The action to be executed when the confirmation is confirmed.</param>
    /// <param name="onCancel">The optional action to be executed when the confirmation is canceled.</param>
    public async Task ShowConfirmationDialog(string title, string contentText, Func<Task> onConfirm, Func<Task>? onCancel = null)
    {
        var parameters = new DialogParameters
        {
            { nameof(ConfirmationDialog.ContentText), contentText }
        };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
        var dialog = await _dialogService.ShowAsync<ConfirmationDialog>(title, parameters, options);
        var result = await dialog.Result;
        if (result is not null && !result.Canceled)
        {
            await onConfirm();
        }
        else if (onCancel != null)
        {
            await onCancel();
        }
    }

    public async Task ShowDialogAsync<T>(
        string title,
        DialogParameters<T> parameters,
        DialogOptions? options = null,
        Func<DialogResult, Task>? onConfirm = null,
        Func<Task>? onCancel = null) where T : ComponentBase
    {
        options = options ?? new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
        var dialog = await _dialogService.ShowAsync<T>(title, parameters, options);
        var result = await dialog.Result;
        if (result is not null && !result.Canceled && onConfirm is not null)
        {
            await onConfirm(result);
        }
        else if (result is not null && result.Canceled && onCancel is not null)
        {
            await onCancel();
        }
    }
}

