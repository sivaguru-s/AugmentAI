import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'epay-page-not-found',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="not-found-container">
      <div class="not-found-content">
        <h1>404</h1>
        <h2>Page Not Found</h2>
        <p>The page you are looking for does not exist or has been moved.</p>
        <a routerLink="/" class="btn-adr">Return to Home</a>
      </div>
    </div>
  `,
  styles: [`
    .not-found-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 60vh;
      text-align: center;
    }
    
    .not-found-content {
      h1 {
        font-size: 96px;
        color: #dc6902;
        margin: 0;
        line-height: 1;
      }
      
      h2 {
        font-size: 24px;
        color: #333;
        margin: 16px 0;
      }
      
      p {
        color: #666;
        margin-bottom: 24px;
      }
      
      .btn-adr {
        display: inline-block;
        padding: 12px 24px;
        background: #dc6902;
        color: #fff;
        text-decoration: none;
        border-radius: 4px;
        
        &:hover {
          background: #b55800;
        }
      }
    }
  `]
})
export class PageNotFoundComponent {}

