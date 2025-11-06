import React from 'react';
import ReactDOM from 'react-dom/client';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import { PRList } from './pages/PRList';
import { PRDetail } from './pages/PRDetail';
import { Dashboard } from './pages/Dashboard';
import { Layout } from './components/Layout';
import './styles.css';
import './App.css';
const router = createBrowserRouter([
    {
        path: '/',
        element: <Layout />,
        children: [
            { index: true, element: <Dashboard /> },
            { path: 'prs', element: <PRList /> },
            { path: 'prs/:id', element: <PRDetail /> },
        ],
    },
]);
const qc = new QueryClient();
ReactDOM.createRoot(document.getElementById('root')!).render(
    <React.StrictMode>
        <QueryClientProvider client={qc}>
            <RouterProvider router={router} />
        </QueryClientProvider>
    </React.StrictMode>
);