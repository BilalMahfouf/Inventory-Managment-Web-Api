import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useToast } from '@shared/context/ToastContext';
import { useTranslation } from 'react-i18next';
import { queryKeys } from '@shared/lib/queryKeys';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import {
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
} from '@features/sales/services/salesOrderApi';

/**
 * Hook for fetching paginated list of orders with filters
 *
 * @param {Object} filters - Filter parameters
 * @param {string} filters.status - Filter by order status
 * @param {string} filters.customerId - Filter by customer ID
 * @param {string} filters.dateFrom - Filter by start date
 * @param {string} filters.dateTo - Filter by end date
 * @param {number} filters.pageNumber - Page number (default: 1)
 * @param {number} filters.pageSize - Items per page (default: 10)
 */
export function useOrders(filters = {}) {
    return useQuery({
        queryKey: queryKeys.sales.orders(filters),
        queryFn: () => getOrders(filters),
    });
}

/**
 * Hook for fetching a single order by ID
 *
 * @param {string|number} id - Order ID
 */
export function useOrder(id) {
    return useQuery({
        queryKey: queryKeys.sales.detail(id),
        queryFn: () => getOrderById(id),
        enabled: !!id,
    });
}

/**
 * Hook for creating a new order
 */
export function useCreateOrder() {
    const queryClient = useQueryClient();
    const { showSuccess, showError } = useToast();
    const { t } = useTranslation();

    return useMutation({
        mutationFn: createOrder,
        onSuccess: result => {
            if (result.success) {
                showSuccess(t(i18nKeyContainer.sales.orders.toasts.createSuccess));
                queryClient.invalidateQueries({ queryKey: queryKeys.sales.all });
            } else {
                showError(
                    t(i18nKeyContainer.sales.orders.toasts.createError),
                    result.error
                );
            }
        },
        onError: error => {
            showError(
                t(i18nKeyContainer.sales.orders.toasts.createError),
                error.message
            );
        },
    });
}

/**
 * Hook for updating an existing order
 */
export function useUpdateOrder() {
    const queryClient = useQueryClient();
    const { showSuccess, showError } = useToast();
    const { t } = useTranslation();

    return useMutation({
        mutationFn: ({ id, data }) => updateOrder(id, data),
        onSuccess: (result, variables) => {
            if (result.success) {
                showSuccess(t(i18nKeyContainer.sales.orders.toasts.updateSuccess));
                queryClient.invalidateQueries({ queryKey: queryKeys.sales.all });
                queryClient.invalidateQueries({
                    queryKey: queryKeys.sales.detail(variables.id),
                });
            } else {
                showError(
                    t(i18nKeyContainer.sales.orders.toasts.updateError),
                    result.error
                );
            }
        },
        onError: error => {
            showError(
                t(i18nKeyContainer.sales.orders.toasts.updateError),
                error.message
            );
        },
    });
}

/**
 * Hook for order status transitions
 * Handles all status changes: confirm, transit, ship, complete, cancel, return
 *
 * @example
 * const transition = useOrderTransition();
 * transition.mutate({ orderId: 123, action: 'confirm' });
 * transition.mutate({ orderId: 123, action: 'ship', payload: { trackingNumber: 'ABC123' } });
 */
export function useOrderTransition() {
    const queryClient = useQueryClient();
    const { showSuccess, showError } = useToast();
    const { t } = useTranslation();

    const actionMap = {
        confirm: confirmOrder,
        transit: transitOrder,
        ship: shipOrder,
        complete: completeOrder,
        cancel: cancelOrder,
        return: returnOrder,
    };

    const toastKeys = {
        confirm: {
            success: i18nKeyContainer.sales.orders.toasts.confirmSuccess,
            error: i18nKeyContainer.sales.orders.toasts.confirmError,
        },
        transit: {
            success: i18nKeyContainer.sales.orders.toasts.transitSuccess,
            error: i18nKeyContainer.sales.orders.toasts.transitError,
        },
        ship: {
            success: i18nKeyContainer.sales.orders.toasts.shipSuccess,
            error: i18nKeyContainer.sales.orders.toasts.shipError,
        },
        complete: {
            success: i18nKeyContainer.sales.orders.toasts.completeSuccess,
            error: i18nKeyContainer.sales.orders.toasts.completeError,
        },
        cancel: {
            success: i18nKeyContainer.sales.orders.toasts.cancelSuccess,
            error: i18nKeyContainer.sales.orders.toasts.cancelError,
        },
        return: {
            success: i18nKeyContainer.sales.orders.toasts.returnSuccess,
            error: i18nKeyContainer.sales.orders.toasts.returnError,
        },
    };

    return useMutation({
        mutationFn: async ({ orderId, action, payload }) => {
            const actionFn = actionMap[action];
            if (!actionFn) {
                throw new Error(`Unknown action: ${action}`);
            }
            return actionFn(orderId, payload);
        },
        onSuccess: (result, variables) => {
            const { action, orderId } = variables;
            const keys = toastKeys[action];

            if (result.success) {
                showSuccess(t(keys.success));
                queryClient.invalidateQueries({ queryKey: queryKeys.sales.all });
                queryClient.invalidateQueries({
                    queryKey: queryKeys.sales.detail(orderId),
                });
            } else {
                showError(t(keys.error), result.error);
            }
        },
        onError: (error, variables) => {
            const { action } = variables;
            const keys = toastKeys[action];
            showError(t(keys.error), error.message);
        },
    });
}
