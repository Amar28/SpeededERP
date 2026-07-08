import { expect, test } from '@playwright/test';

test('login page renders the email/password form', async ({ page }) => {
  await page.goto('/login');

  await expect(page.getByLabel('Adresse e-mail')).toBeVisible();
  await expect(page.getByLabel('Mot de passe')).toBeVisible();
  await expect(page.getByRole('button', { name: 'Se connecter' })).toBeVisible();
});

test('unauthenticated visitors are redirected from the dashboard to /login', async ({ page }) => {
  await page.goto('/');

  await expect(page).toHaveURL(/\/login$/);
});
