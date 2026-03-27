import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'invoices'
  },
  {
    path: 'invoices',
    loadChildren: () =>
      import('./features/invoices/invoices.module').then(m => m.InvoicesModule),
    data: {
      appEntry: 'epay',
      title: 'Invoice Search'
    }
  },
  {
    path: '404',
    loadComponent: () => import('./shared/components/page-not-found/page-not-found.component')
      .then(m => m.PageNotFoundComponent),
    data: { title: 'Page Not Found' }
  },
  { path: '**', redirectTo: '404' }
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, {
      scrollPositionRestoration: 'top',
      onSameUrlNavigation: 'reload'
    }),
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }

