using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TestReoGrid.Models;
using TestReoGrid.ValidationRules;

namespace TestReoGrid.Helpers
{
    public class ValidateHelper
    {

        public static ReadOnlyDictionary<string, ValidationRule> SoluRuleDict = new(new Dictionary<string, ValidationRule>
        {
            [nameof(PL_Exp_Dsgn_Inject.Solution_Name)] = CreateValidationRule(nameof(PL_Exp_Dsgn_Inject.Solution_Name)),
            [nameof(PL_Exp_Dsgn_Inject.Concentration)] = CreateValidationRule(nameof(PL_Exp_Dsgn_Inject.Concentration)),
            [nameof(PL_Exp_Dsgn_Inject.Molecular_Weight)] = CreateValidationRule(nameof(PL_Exp_Dsgn_Inject.Molecular_Weight)),
            [nameof(PL_Exp_Dsgn_Inject.Mass_concentration)] = CreateValidationRule(nameof(PL_Exp_Dsgn_Inject.Mass_concentration)),
            [nameof(PL_Exp_Dsgn_Inject.Info)] = CreateValidationRule(nameof(PL_Exp_Dsgn_Inject.Info)),
        });

        public static ValidationRule CreateValidationRule(string propName)
        {
            ValidationRule rule = null;
            if (propName is { Length: > 0 })
            {
                switch (propName)
                {
                    case nameof(PL_Exp_Dsgn_Inject.Solution_Name):
                        rule = (ValidationRule)Application.Current.Resources["Name"];
                        break;

                    case nameof(PL_Exp_Dsgn_Inject.Concentration):
                        rule = (ValidationRule)Application.Current.Resources["Conc"];
                        break;

                    case nameof(PL_Exp_Dsgn_Inject.Molecular_Weight):
                        rule = (ValidationRule)Application.Current.Resources["MW"];
                        break;

                    case nameof(PL_Exp_Dsgn_Inject.Mass_concentration):
                        rule = (ValidationRule)Application.Current.Resources["MassConc"];
                        break;

                    case nameof(PL_Exp_Dsgn_Inject.Info):
                        rule = (ValidationRule)Application.Current.Resources["Info"];
                        break;
                }
            }
            return rule;
        }



    }
}
