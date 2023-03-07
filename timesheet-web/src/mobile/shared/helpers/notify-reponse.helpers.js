export const getMessage = (err) => {
    if (err.status === 403) {
        return 'Bạn không có quyền thực hiện chức năng này.';
    }
    if (err.ResponseStatus) {
        return err.ResponseStatus.Message;
    }
    if (err.data && err.data.ResponseStatus) {
        return err.data.ResponseStatus.Message;
    } else if (err.data && typeof err.data == 'string') {
        return err.data;
    } else if (err.message) {
        return err.message;
    } else if (err && err.msg) {
        return err.msg;
    } else {
        return '';
    }
};

export const getErrorCode = (err) => {
    let errors = [];
    if (err.responseStatus) {
        err.errors = err.errors || [err.responseStatus];
    }

    if (err && err.errors && angular.isArray(err.errors) && err.errors.length > 0) {
        errors = [err.errors[0]];
    } else {
        errors = [err.errors];
    }

    return errors[0].code || errors[0].errorCode;
};

export const getErrorMsg = (err) => {
    // Only take first error
    // Reason: request from BA cause for same bussiness with old system
    // Ticket: #69
    let message = '';
    if (err.responseStatus) {
        err.errors = err.errors || [err.responseStatus];
    }
    if (err.errors && angular.isArray(err.errors) && err.errors.length > 0) {
        message = err.errors[0].message;
    } else {
        message = err.errors;
    }

    return message;
};
