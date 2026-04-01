/**
 * Sales Order Constants
 *
 * Centralized constants for order statuses, colors, and status transitions.
 * Used throughout the sales order feature for consistency.
 */

export const ORDER_STATUS = {
    Pending: 'Pending',
    Confirmed: 'Confirmed',
    InTransit: 'InTransit',
    Shipped: 'Shipped',
    Completed: 'Completed',
    Cancelled: 'Cancelled',
    Returned: 'Returned',
};

export const STATUS_COLORS = {
    Pending: 'yellow',
    Confirmed: 'blue',
    InTransit: 'orange',
    Shipped: 'purple',
    Completed: 'green',
    Cancelled: 'red',
    Returned: 'gray',
};

export const STATUS_BADGE_CLASSES = {
    Pending: 'bg-yellow-100 text-yellow-800 border-yellow-200',
    Confirmed: 'bg-blue-100 text-blue-800 border-blue-200',
    InTransit: 'bg-orange-100 text-orange-800 border-orange-200',
    Shipped: 'bg-purple-100 text-purple-800 border-purple-200',
    Completed: 'bg-green-100 text-green-800 border-green-200',
    Cancelled: 'bg-red-100 text-red-800 border-red-200',
    Returned: 'bg-gray-100 text-gray-800 border-gray-200',
};

export const PAYMENT_STATUS = {
    Unpaid: 'Unpaid',
    PartiallyPaid: 'PartiallyPaid',
    Paid: 'Paid',
};

export const PAYMENT_STATUS_COLORS = {
    Unpaid: 'red',
    PartiallyPaid: 'yellow',
    Paid: 'green',
};

export const PAYMENT_STATUS_BADGE_CLASSES = {
    Unpaid: 'bg-red-100 text-red-800 border-red-200',
    PartiallyPaid: 'bg-yellow-100 text-yellow-800 border-yellow-200',
    Paid: 'bg-green-100 text-green-800 border-green-200',
};

// Actions allowed per status
export const STATUS_TRANSITIONS = {
    Pending: ['confirm', 'cancel'],
    Confirmed: ['transit', 'cancel'],
    InTransit: ['ship', 'cancel'],
    Shipped: ['complete'],
    Completed: ['return'],
    Cancelled: [],
    Returned: [],
};

// Action metadata for button rendering
export const ACTION_CONFIG = {
    confirm: {
        labelKey: 'sales.orders.actions.confirm',
        variant: 'default',
        requiresConfirmation: true,
    },
    transit: {
        labelKey: 'sales.orders.actions.markInTransit',
        variant: 'default',
        requiresConfirmation: true,
    },
    ship: {
        labelKey: 'sales.orders.actions.ship',
        variant: 'default',
        requiresConfirmation: true,
        requiresTrackingInput: true,
    },
    complete: {
        labelKey: 'sales.orders.actions.complete',
        variant: 'default',
        requiresConfirmation: true,
    },
    cancel: {
        labelKey: 'sales.orders.actions.cancel',
        variant: 'destructive',
        requiresConfirmation: true,
    },
    return: {
        labelKey: 'sales.orders.actions.return',
        variant: 'destructive',
        requiresConfirmation: true,
    },
};
