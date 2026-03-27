import { Component, Input } from '@angular/core';

@Component({
  selector: 'epay-nav',
  templateUrl: './epay-nav.component.html',
  styleUrls: ['./epay-nav.component.scss'],
  standalone: false
})
export class EpayNavComponent {
  @Input() isAnalyst: boolean | null = false;

  navItems = [
    { label: 'Main', route: '/main', showForAll: true },
    { label: 'History', route: '/history', showForAll: true },
    { label: 'Analyst Report', route: '/analyst-report', showForAll: false },
    { label: 'User List', route: '/user-list', showForAll: false },
    { label: 'Admin Maintenance', route: '/admin', showForAll: false }
  ];

  getVisibleNavItems() {
    return this.navItems.filter(item => 
      item.showForAll || this.isAnalyst
    );
  }
}

