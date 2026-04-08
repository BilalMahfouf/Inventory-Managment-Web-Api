import { api, extractErrorMessage } from '@shared/services/api/api';

const URL = '/sales-orders';

function toFiniteNumber(value, fallback = 0) {
  const num = Number(value);
  return Number.isFinite(num) ? num : fallback;
}

function mapOrderItemFromApi(item) {
  if (!item || typeof item !== 'object') {
    return {
      productId: null,
      locationId: null,
      quantity: 0,
      unitPrice: 0,
      totalPrice: 0,
    };
  }

  const quantity = toFiniteNumber(item.quantity ?? item.orderedQuantity, 0);
  const unitPrice = toFiniteNumber(item.unitPrice ?? item.unitCost, 0);
  const totalPrice = toFiniteNumber(item.totalPrice, quantity * unitPrice);

  return {
    ...item,
    quantity,
    unitPrice,
    totalPrice,
  };
}

function mapOrderListItemFromApi(item) {
  if (!item || typeof item !== 'object') {
    return item;
  }

  const normalizedStatus = item.status ?? item.salesStatus ?? '';
  const normalizedTotal = toFiniteNumber(item.totalAmount ?? item.total, 0);
  const normalizedItems = toFiniteNumber(item.items ?? item.itemCount, 0);

  return {
    ...item,
    orderNumber: item.orderNumber ?? item.id,
    status: normalizedStatus,
    salesStatus: item.salesStatus ?? normalizedStatus,
    totalAmount: normalizedTotal,
    total: normalizedTotal,
    items: normalizedItems,
    itemCount: normalizedItems,
  };
}

function mapOrderDetailFromApi(order) {
  if (!order || typeof order !== 'object') {
    return order;
  }

  const rawItems = Array.isArray(order.items)
    ? order.items
    : Array.isArray(order.item)
      ? order.item
      : [];

  const status = order.status ?? order.salesStatus ?? '';
  const totalAmount = toFiniteNumber(order.totalAmount ?? order.total, 0);

  return {
    ...order,
    orderNumber: order.orderNumber ?? order.id,
    status,
    salesStatus: order.salesStatus ?? status,
    notes: order.notes ?? order.description ?? null,
    description: order.description ?? order.notes ?? null,
    totalAmount,
    total: totalAmount,
    items: rawItems.map(mapOrderItemFromApi),
  };
}

function mapPagedOrdersResponse(payload) {
  if (!payload || typeof payload !== 'object') {
    return {
      item: [],
      totalCount: 0,
      page: 1,
      pageSize: 10,
    };
  }

  const rawItems = Array.isArray(payload.item)
    ? payload.item
    : Array.isArray(payload.items)
      ? payload.items
      : [];

  const mappedItems = rawItems.map(mapOrderListItemFromApi);

  return {
    ...payload,
    item: mappedItems,
    items: mappedItems,
    totalCount: toFiniteNumber(payload.totalCount, mappedItems.length),
    page: toFiniteNumber(payload.page, 1),
    pageSize: toFiniteNumber(payload.pageSize, mappedItems.length || 10),
  };
}

function mapOrderItemsPayload(items) {
  if (!Array.isArray(items)) {
    return [];
  }

  return items.map(item => ({
    productId: toFiniteNumber(item?.productId),
    locationId: toFiniteNumber(item?.locationId),
    quantity: toFiniteNumber(item?.quantity),
  }));
}

function mapCreateOrderPayloadToApi(orderData) {
  return {
    customerId: orderData?.isWalkIn ? null : (orderData?.customerId ?? null),
    description: orderData?.description ?? orderData?.notes ?? null,
    isWalkIn: Boolean(orderData?.isWalkIn),
    shippingAddress: orderData?.shippingAddress ?? null,
    paymentStatus: toFiniteNumber(orderData?.paymentStatus, 1),
    items: mapOrderItemsPayload(orderData?.items),
  };
}

function mapUpdateOrderPayloadToApi(orderData) {
  const payload = {
    customerId: orderData?.customerId ?? null,
    description: orderData?.description ?? orderData?.notes ?? null,
    shippingAddress: orderData?.shippingAddress ?? null,
  };

  if (Array.isArray(orderData?.items)) {
    payload.items = mapOrderItemsPayload(orderData.items);
  }

  return payload;
}

function normalizeCreatedOrderResponse(data) {
  const id =
    typeof data === 'number'
      ? data
      : toFiniteNumber(data?.id ?? data?.value ?? data?.data?.id, 0);

  return {
    id: id > 0 ? id : null,
  };
}

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
    return { success: true, data: mapPagedOrdersResponse(data) };
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
    return { success: true, data: mapOrderDetailFromApi(data) };
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
    const payload = mapCreateOrderPayloadToApi(orderData);
    const { data } = await api.post(URL, payload);
    return { success: true, data: normalizeCreatedOrderResponse(data) };
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
    const payload = mapUpdateOrderPayloadToApi(orderData);
    const { data } = await api.put(`${URL}/${id}`, payload);
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
