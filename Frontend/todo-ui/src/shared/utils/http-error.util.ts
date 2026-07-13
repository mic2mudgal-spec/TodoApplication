export function getErrorMessage(error: any): string {
    if (!error) {
        return 'Unexpected error occurred.';
    }

    if (error.error?.message) {
        return error.error.message;
    }

    if (error.error?.Message) {
        return error.error.Message;
    }

    if (typeof error.error === 'string') {
        return error.error;
    }

    if (error.message) {
        return error.message;
    }

    return 'Unexpected error occurred.';
}
