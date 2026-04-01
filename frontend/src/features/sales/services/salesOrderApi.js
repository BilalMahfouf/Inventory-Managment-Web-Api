import { api, extractErrorMessage } from '@shared/services/api/api';

const URL = '/sales-orders';

async function getOrdersDahsobardSummary() {
    try {
        const { data } = await api.get(`${URL}/summary`);
        return { isSuccess: true, data };
    } catch (e) {
        return {
            isSuccess: false,
            exception: extractErrorMessage(e, 'Failed to fetch sales summary'),
        };
    }
}

async function getOrders({
    status,
    customerId,
    dateFrom,
    dateTo,
    pageNumber = 1,
    pageSize = 10,
}) {
    try {
        const params = new URLSearchParams();
        params.append('pageNumber', pageNumber);
        params.append('pageSize', pageSize);
        if (status) params.append('status', status);
        if (customerId) params.append('customerId', customerId);
        if (dateFrom) params.append('dateFrom', dateFrom);
        if (dateTo) params.append('dateTo', dateTo);

        const { data } = await api.get(`${URL}?${params.toString()}`);
        return { success: true, data };
    } catch (error) {
        console.error(error);
        return {
            success: false,
            error: extractErrorMessage(error, 'Failed to fetch orders'),
        };
    }
}

async function getOrderById(id) {
    try {
        const { data } = await api.get(`${URL}/${id}`);
        return { success: true, data };
    } catch (error) {
        console.error(error);
        return {
            success: false,
            error: extractErrorMessage(error, 'Failed to fetch order'),
        };
    }
}

async function createOrder(orderData) {
    try {
        const { data } = await api.post(URL, orderData);
        return { success: true, data };
    } catch (error) {
        console.error(error);
        return {
            success: false,
            error: extractErrorMessage(error, 'Failed to create order'),
        };
    }
}

async function updateOrder(id, orderData) {
    try {
        const { data } = await api.put(`${URL}/${id}`, orderData);
        return { success: true, data };
    } catch (error) {
        console.error(error);
        return {
            success: false,
            error: extractErrorMessage(error, 'Failed to update order'),
        };
    }
}

async function confirmOrder(id) {
    try {
        const { data } = await api.post(`${URL}/${id}/confirm`);
        return { success: true, data };
    } catch (error) {
        console.error(error);
        return {
            success: false,
            error: extractErrorMessage(error, 'Failed to confirm order'),
        };
    }
}

async function transitOrder(id) {
    try {
        const { data } = await api.post(`${URL}/${id}/transit`);
        return { success: true, data };
    } catch (error) {
        console.error(error);
        return {
            success: false,
            error: extractErrorMessage(error, 'Failed to mark order in transit'),
        };
    }
}

async function shipOrder(id, payload = {}) {
    try {
        const { data } = await api.post(`${URL}/${id}/ship`, payload);
        return { success: true, data };
    } catch (error) {
        console.error(error);
        return {
            success: false,
            error: extractErrorMessage(error, 'Failed to ship order'),
        };
    }
}

async function completeOrder(id) {
    try {
        const { data } = await api.post(`${URL}/${id}/complete`);
        return { success: true, data };
    } catch (error) {
        console.error(error);
        return {
            success: false,
            error: extractErrorMessage(error, 'Failed to complete order'),
        };
    }
}

async function cancelOrder(id) {
    try {
        const { data } = await api.post(`${URL}/${id}/cancel`);
        return { success: true, data };
    } catch (error) {
        console.error(error);
        return {
            success: false,
            error: extractErrorMessage(error, 'Failed to cancel order'),
        };
    }
}

async function returnOrder(id) {
    try {
        const { data } = await api.post(`${URL}/${id}/return`);
        return { success: true, data };
    } catch (error) {
        console.error(error);
        return {
            success: false,
            error: extractErrorMessage(error, 'Failed to return order'),
        };
    }
}

export {
    getOrdersDahsobardSummary,
    getOrders,
    getOrderById,
    createOrder,
    updateOrder,
    confirmOrder,
    transitOrder,
    shipOrder,
    completeOrder,
    cancelOrder,
    returnOrder,
};
