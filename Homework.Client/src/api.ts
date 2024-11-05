import axios from 'axios';
import { API_BASE_URL, PRODUCTS_ENDPOINT } from './config';

const api = axios.create({
    baseURL: API_BASE_URL,
});

/**
 * Fetches the list of products from the API.
 * @returns {Promise<Product[]>} A promise that resolves to an array of products.
 * @throws Will throw an error if the request fails.
 */
export const fetchProducts = async () => {
    try {
        const response = await api.get(PRODUCTS_ENDPOINT);
        return response.data;
    } catch (error) {
        console.error('Error fetching products:', error);
        throw error;
    }
};
