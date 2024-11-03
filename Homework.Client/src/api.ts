import axios from 'axios';

const api = axios.create({
    baseURL: 'http://localhost:5153/api',
});

export const fetchProducts = async () => {
    const response = await api.get('/products');
    return response.data;
};
