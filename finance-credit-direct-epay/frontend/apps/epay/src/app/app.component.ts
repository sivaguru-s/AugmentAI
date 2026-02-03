import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { Router, NavigationEnd, RoutesRecognized, ActivationEnd } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { filter, first } from 'rxjs/operators';

@Component({
  selector: 'epay-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  standalone: false
})
export class AppComponent implements OnInit, AfterViewInit {
  @ViewChild('headerRef') headerRef!: ElementRef;
  @ViewChild('wrapperRef') wrapperRef!: ElementRef;
  
  appName: string = 'epay';
  isMenuOpen: boolean = false;
  isLoading: boolean = false;
  currentYear: number = new Date().getFullYear();

  constructor(
    private router: Router,
    private titleService: Title
  ) {}

  ngOnInit(): void {
    const appTitle = 'Ashley Direct | ';

    this.router.events.subscribe((data) => {
      if (data instanceof RoutesRecognized) {
        const defaultPageTitle = this.titleService.getTitle();
        const title = data.state.root.firstChild?.data 
          ? data.state.root.firstChild.data['title'] 
          : defaultPageTitle;
        this.titleService.setTitle(appTitle + title);
      }
    });
  }

  ngAfterViewInit(): void {
    // Setup navigation reference if needed
  }

  toggleMenu(): void {
    this.isMenuOpen = !this.isMenuOpen;
  }
}

