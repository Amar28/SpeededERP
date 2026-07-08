import { QueryClientProvider } from '@tanstack/react-query';
import { ThemeModeProvider } from '@speedederp/ui-kit';
import { StrictMode, Suspense } from 'react';
import { createRoot } from 'react-dom/client';
import { I18nextProvider } from 'react-i18next';

import { App } from './App';
import { i18n } from './i18n';
import { queryClient } from './lib/queryClient';

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <Suspense fallback={null}>
      <I18nextProvider i18n={i18n}>
        <ThemeModeProvider>
          <QueryClientProvider client={queryClient}>
            <App />
          </QueryClientProvider>
        </ThemeModeProvider>
      </I18nextProvider>
    </Suspense>
  </StrictMode>,
);
