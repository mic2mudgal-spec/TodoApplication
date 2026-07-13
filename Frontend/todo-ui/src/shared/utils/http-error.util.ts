export function getErrorMessage(error: any): string {

    if (!error) {
        return 'Unexpected error occurred.';
    }

    // Custom API response
    if (error.error?.message) {
        return error.error.message;
    }

    if (error.error?.Message) {
        return error.error.Message;
    }

    // ASP.NET Core ValidationProblemDetails
    if (error.error?.errors) {

        const errors = error.error.errors;

        const messages: string[] = [];

        Object.keys(errors).forEach(key => {
            messages.push(...errors[key]);
        });

        return messages.join(', ');
    }

    if (typeof error.error === 'string') {
        return error.error;
    }

    if (error.message) {
        return error.message;
    }

    return 'Unexpected error occurred.';
}