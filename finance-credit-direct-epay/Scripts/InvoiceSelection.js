/**
 * EPay Invoice Selection State Management
 * 
 * This script manages invoice checkbox selections without relying on ViewState.
 * Selected invoice data is stored in a hidden field and synchronized on every checkbox change.
 * 
 * Copyright © 2025 Ashley Furniture Industries, Inc. All rights reserved.
 */

(function () {
    'use strict';

    var InvoiceSelection = {
        // Configuration
        config: {
            hiddenFieldId: null,        // Set during initialization
            gridId: null,               // Set during initialization
            checkboxSelector: 'input[id*="chkSelect"]:not([id*="SelectAll"])',
            selectAllSelector: 'input[id*="chkSelectAll"]',
            payInfoSelector: 'input[id*="hdnGridPayInfo"]',
            delimiter: '|'
        },

        /**
         * Initialize the invoice selection manager
         * @param {string} hiddenFieldId - Client ID of the hidden field to store selections
         * @param {string} gridId - Client ID of the GridView
         */
        init: function (hiddenFieldId, gridId) {
            this.config.hiddenFieldId = hiddenFieldId;
            this.config.gridId = gridId;

            // Attach event handlers
            this.attachEventHandlers();

            // Restore selections from hidden field on page load
            this.restoreSelections();

            console.log('InvoiceSelection initialized - HiddenField:', hiddenFieldId, 'Grid:', gridId);
        },

        /**
         * Attach event handlers to checkboxes
         */
        attachEventHandlers: function () {
            var self = this;

            // Individual checkbox change
            $(document).on('change', this.config.checkboxSelector, function () {
                self.updateHiddenField();
                console.log('Checkbox changed - Updated selections');
            });

            // Select All checkbox change
            $(document).on('change', this.config.selectAllSelector, function () {
                var isChecked = $(this).is(':checked');
                self.selectAll(isChecked);
                console.log('Select All clicked - Checked:', isChecked);
            });
        },

        /**
         * Get all selected invoice data
         * @returns {Array} Array of invoice data strings (format: "InvoiceNo~CustomerNo~ShipTo")
         */
        getSelectedInvoices: function () {
            var selected = [];

            $(this.config.checkboxSelector).each(function () {
                if ($(this).is(':checked') && $(this).is(':enabled')) {
                    var row = $(this).closest('tr');
                    var payInfo = row.find(InvoiceSelection.config.payInfoSelector).val();

                    if (payInfo && payInfo.trim() !== '') {
                        selected.push(payInfo);
                    }
                }
            });

            return selected;
        },

        /**
         * Update the hidden field with current selections
         */
        updateHiddenField: function () {
            var selected = this.getSelectedInvoices();
            var hiddenField = $('#' + this.config.hiddenFieldId);

            if (hiddenField.length > 0) {
                hiddenField.val(selected.join(this.config.delimiter));
                console.log('Hidden field updated - Count:', selected.length, 'Data:', hiddenField.val());
            } else {
                console.error('Hidden field not found:', this.config.hiddenFieldId);
            }
        },

        /**
         * Restore checkbox selections from hidden field
         * Called on page load to restore state after postback
         */
        restoreSelections: function () {
            var hiddenField = $('#' + this.config.hiddenFieldId);

            if (hiddenField.length === 0) {
                console.warn('Hidden field not found for restoration:', this.config.hiddenFieldId);
                return;
            }

            var selectedData = hiddenField.val();
            if (!selectedData || selectedData.trim() === '') {
                console.log('No selections to restore');
                return;
            }

            var selections = selectedData.split(this.config.delimiter);
            var restoredCount = 0;

            // Loop through all checkboxes and check those that match
            $(this.config.checkboxSelector).each(function () {
                var row = $(this).closest('tr');
                var payInfo = row.find(InvoiceSelection.config.payInfoSelector).val();

                if (payInfo && selections.indexOf(payInfo) !== -1) {
                    $(this).prop('checked', true);
                    restoredCount++;
                }
            });

            console.log('Selections restored - Count:', restoredCount, 'of', selections.length);
        },

        /**
         * Select or deselect all enabled checkboxes
         * @param {boolean} checked - True to select all, false to deselect all
         */
        selectAll: function (checked) {
            $(this.config.checkboxSelector).each(function () {
                if ($(this).is(':enabled')) {
                    $(this).prop('checked', checked);
                }
            });

            this.updateHiddenField();
        },

        /**
         * Get count of selected invoices
         * @returns {number} Number of selected invoices
         */
        getSelectedCount: function () {
            return this.getSelectedInvoices().length;
        },

        /**
         * Check if any invoices are selected
         * @returns {boolean} True if at least one invoice is selected
         */
        hasSelections: function () {
            return this.getSelectedCount() > 0;
        },

        /**
         * Clear all selections
         */
        clearSelections: function () {
            $(this.config.checkboxSelector).prop('checked', false);
            $(this.config.selectAllSelector).prop('checked', false);
            this.updateHiddenField();
            console.log('All selections cleared');
        }
    };

    // Expose to global scope
    window.InvoiceSelection = InvoiceSelection;

})();

